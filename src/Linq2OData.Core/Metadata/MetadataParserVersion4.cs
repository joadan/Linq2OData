using System.Xml.Linq;

namespace Linq2OData.Core.Metadata;

internal static class MetadataParserVersion4
{
    private static string StripNamespace(string typeName)
       => typeName.Contains('.') ? typeName.Split('.').Last() : typeName;

    /// <summary>
    /// Builds a map from schema alias to full namespace (e.g. "Core" → "Company.Core").
    /// </summary>
    private static Dictionary<string, string> BuildAliasMap(IEnumerable<XElement> schemas)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var schema in schemas)
        {
            var ns = schema.Attribute("Namespace")?.Value;
            var alias = schema.Attribute("Alias")?.Value;
            if (!string.IsNullOrEmpty(alias) && !string.IsNullOrEmpty(ns))
                map[alias] = ns;
        }
        return map;
    }

    /// <summary>
    /// Resolves an alias-qualified type name to a fully-qualified one
    /// (e.g. "Core.Person" → "Company.Core.Person" when alias "Core" maps to "Company.Core").
    /// Returns the original string unchanged if no alias applies.
    /// </summary>
    private static string ResolveTypeName(string typeName, Dictionary<string, string> aliasMap)
    {
        if (string.IsNullOrEmpty(typeName) || aliasMap.Count == 0)
            return typeName;

        var dotIndex = typeName.IndexOf('.');
        if (dotIndex <= 0) return typeName;

        var prefix = typeName.Substring(0, dotIndex);
        if (aliasMap.TryGetValue(prefix, out var fullNamespace))
            return fullNamespace + typeName.Substring(dotIndex);

        return typeName;
    }

    internal static ODataMetadata Parse(XDocument doc)
    {
        var metadata = new ODataMetadata
        {
            ODataVersion = ODataVersion.V4
        };

        XNamespace edmx = "http://docs.oasis-open.org/odata/ns/edmx";
        XNamespace edm = "http://docs.oasis-open.org/odata/ns/edm";

        var xmlSchemas = doc.Descendants(edm + "Schema").ToList();
        if (xmlSchemas.Count == 0)
            return metadata;

        // Build alias→namespace map from all schemas
        var aliasMap = BuildAliasMap(xmlSchemas);

        // Create one ODataSchema per XML <Schema> element, populating types/enums
        var odataSchemas = new List<ODataSchema>();
        foreach (var xmlSchema in xmlSchemas)
        {
            var schemaNamespace = xmlSchema.Attribute("Namespace")?.Value ?? string.Empty;
            var odataSchema = new ODataSchema { Namespace = schemaNamespace };

            foreach (var enumType in ParseEnumTypes(xmlSchema, edm))
            {
                enumType.SchemaNamespace = schemaNamespace;
                odataSchema.EnumTypes.Add(enumType);
            }

            foreach (var entityType in ParseEntityTypes(xmlSchema, edm, aliasMap))
            {
                entityType.SchemaNamespace = schemaNamespace;
                odataSchema.EntityTypes.Add(entityType);
            }

            foreach (var complexType in ParseComplexTypes(xmlSchema, edm))
            {
                complexType.SchemaNamespace = schemaNamespace;
                odataSchema.EntityTypes.Add(complexType);
            }

            odataSchemas.Add(odataSchema);
        }

        // Collect all entity types across schemas for cross-schema lookups
        var allEntityTypes = odataSchemas.SelectMany(s => s.EntityTypes).ToList();

        // Parse EntityContainer into the schema that owns it
        var containerXmlSchema = xmlSchemas.FirstOrDefault(s => s.Descendants(edm + "EntityContainer").Any());
        if (containerXmlSchema != null)
        {
            var entityContainer = containerXmlSchema.Descendants(edm + "EntityContainer").FirstOrDefault()!;
            var containerNamespace = containerXmlSchema.Attribute("Namespace")?.Value ?? string.Empty;
            var containerOdataSchema = odataSchemas.First(s => s.Namespace == containerNamespace);

            containerOdataSchema.ContainerName = entityContainer.Attribute("Name")?.Value;
            containerOdataSchema.EntitySets = ParseEntitySets(entityContainer, edm, containerNamespace, allEntityTypes, aliasMap);
            containerOdataSchema.Singletons = ParseSingletons(entityContainer, edm, allEntityTypes, aliasMap);
            containerOdataSchema.Functions = ParseActionImports(entityContainer, edm, containerNamespace, xmlSchemas);

            containerOdataSchema.SetEntityPaths(allEntityTypes);
        }

        MarkEnumProperties(odataSchemas);

        foreach (var odataSchema in odataSchemas)
            metadata.Schemas.Add(odataSchema);

        return metadata;
    }

    private static List<ODataEnumType> ParseEnumTypes(XElement schema, XNamespace edmNamespace)
    {
        var enumTypes = new List<ODataEnumType>();

        foreach (var enumType in schema.Descendants(edmNamespace + "EnumType"))
        {
            var typeName = enumType.Attribute("Name")?.Value;
            if (string.IsNullOrEmpty(typeName))
                continue;

            var enumDef = new ODataEnumType
            {
                Name = typeName
            };

            foreach (var member in enumType.Elements(edmNamespace + "Member"))
            {
                var memberName = member.Attribute("Name")?.Value;
                var memberValue = member.Attribute("Value")?.Value;
                
                if (!string.IsNullOrEmpty(memberName) && int.TryParse(memberValue, out int value))
                {
                    enumDef.Members.Add(new ODataEnumMember
                    {
                        Name = memberName,
                        Value = value
                    });
                }
            }

            enumTypes.Add(enumDef);
        }

        return enumTypes;
    }

    

    private static List<ODataEntityType> ParseComplexTypes(XElement schema, XNamespace edmNamespace)
    {
        var results = new List<ODataEntityType>();

        foreach (var complexType in schema.Descendants(edmNamespace + "ComplexType"))
        {
            var typeName = complexType.Attribute("Name")?.Value;
            if (string.IsNullOrEmpty(typeName))
                continue;

            var entityType = new ODataEntityType
            {
                Name = typeName
            };

            results.Add(entityType);

            foreach (var prop in complexType.Descendants(edmNamespace + "Property"))
            {
                var property = ParseProperty(prop);
                if (property != null)
                    entityType.Properties.Add(property);
            }
        }

        return results;
    }

    private static List<ODataEntityType> ParseEntityTypes(XElement schema, XNamespace edmNamespace, Dictionary<string, string> aliasMap)
    {
        var entityTypes = new List<ODataEntityType>();

        foreach (var entityType in schema.Descendants(edmNamespace + "EntityType"))
        {
            var name = entityType.Attribute("Name")?.Value;
            var baseType = entityType.Attribute("BaseType")?.Value;
            if (string.IsNullOrEmpty(name))
                continue;

            // Resolve alias in BaseType (e.g. "Core.Person" → "Company.Core.Person")
            if (baseType != null)
                baseType = ResolveTypeName(baseType, aliasMap);

            var entity = new ODataEntityType
            {
                Name = name,
                BaseType = baseType
            };


            // Parse properties
            foreach (var prop in entityType.Elements(edmNamespace + "Property"))
            {
                var property = ParseProperty(prop);
                if (property != null)
                    entity.Properties.Add(property);
            }

            // Parse keys
            var keyElement = entityType.Element(edmNamespace + "Key");
            if (keyElement != null)
            {
                var keys = keyElement.Descendants(edmNamespace + "PropertyRef")
                    .Select(pr => pr.Attribute("Name")?.Value)
                    .Where(k => !string.IsNullOrEmpty(k))
                    .ToList()!;

                foreach (var keyName in keys)
                {
                    var prop = entity.Properties.FirstOrDefault(p => p.Name == keyName);
                    prop?.IsKey = true;
                }
            }

            // Parse navigation properties (V4 style - inline Type attribute)
            foreach (var navProp in entityType.Elements(edmNamespace + "NavigationProperty"))
            {
                var navName = navProp.Attribute("Name")?.Value;
                var type = navProp.Attribute("Type")?.Value;

                if (!string.IsNullOrEmpty(navName) && !string.IsNullOrEmpty(type))
                {
                    // Check if it's a collection
                    bool isCollection = type.StartsWith("Collection(");
                    string targetType = type;

                    if (isCollection)
                    {
                        // Extract type from Collection(Namespace.Type)
                        targetType = type.Substring("Collection(".Length, type.Length - "Collection(".Length - 1);
                    }

                    // Extract entity type name from qualified name
                    var entityTypeName = targetType.Contains('.')
                        ? targetType.Split('.').Last()
                        : targetType;

                    entity.Navigations.Add(new ODataNavigation
                    {
                        Name = navName,
                        ToEntity = entityTypeName,
                        NavigationType = isCollection ? ODataNavigationType.Many : ODataNavigationType.ZeroOrOne
                    });
                }
            }

            entityTypes.Add(entity);
        }

        return entityTypes;
    }

    private static ODataProperty? ParseProperty(XElement prop)
    {
        var name = prop.Attribute("Name")?.Value;
        var type = prop.Attribute("Type")?.Value;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
            return null;

        // Check if it's a collection type and extract the inner type
        bool isCollection = type.StartsWith("Collection(") && type.EndsWith(")");
        string dataType = type;
        
        if (isCollection)
        {
            // Extract inner type from Collection(InnerType)
            dataType = type.Substring("Collection(".Length, type.Length - "Collection(".Length - 1);
        }

        var property = new ODataProperty
        {
            Name = name,
            DataType = dataType,
            Nullable = prop.Attribute("Nullable")?.Value?.ToLower() != "false",
            IsCollection = isCollection
        };

        // Parse MaxLength
        if (int.TryParse(prop.Attribute("MaxLength")?.Value, out int maxLength))
            property.MaxLength = maxLength;

        // Parse Precision and Scale
        if (int.TryParse(prop.Attribute("Precision")?.Value, out int precision))
            property.Precision = precision;

        if (int.TryParse(prop.Attribute("Scale")?.Value, out int scale))
            property.Scale = scale;

        // In V4, there are no SAP-specific attributes by default, but we set defaults
        property.Creatable = true;
        property.Updateble = true;
        property.Sortable = true;
        property.Filterable = true;

        return property;
    }

    private static List<ODataEntitySet> ParseEntitySets(XElement entityContainer, XNamespace edmNamespace, string schemaNamespace, List<ODataEntityType> entityTypes, Dictionary<string, string> aliasMap)
    {
        var entitySets = new List<ODataEntitySet>();

        foreach (var entitySet in entityContainer.Elements(edmNamespace + "EntitySet"))
        {
            var name = entitySet.Attribute("Name")?.Value;
            var entityTypeRef = entitySet.Attribute("EntityType")?.Value;

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(entityTypeRef))
            {
                // Resolve alias (e.g. "Core.Employee" → "Company.Core.Employee"), then strip to simple name
                var resolved = ResolveTypeName(entityTypeRef, aliasMap);
                var entityTypeName = resolved.Contains('.')
                    ? resolved.Split('.').Last()
                    : resolved;


                entitySets.Add(new ODataEntitySet
                {
                    Name = name,
                    EntityTypeName = entityTypeName,
                    EntityType = entityTypes.FirstOrDefault(et => et.Name == entityTypeName)!
                });
            }
        }

        return entitySets;
    }

    private static List<ODataSingleton> ParseSingletons(XElement entityContainer, XNamespace edmNamespace, List<ODataEntityType> entityTypes, Dictionary<string, string> aliasMap)
    {
        var singletons = new List<ODataSingleton>();

        foreach (var singleton in entityContainer.Elements(edmNamespace + "Singleton"))
        {
            var name = singleton.Attribute("Name")?.Value;
            var entityTypeRef = singleton.Attribute("Type")?.Value;

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(entityTypeRef))
            {
                var resolved = ResolveTypeName(entityTypeRef, aliasMap);
                var entityTypeName = resolved.Contains('.')
                    ? resolved.Split('.').Last()
                    : resolved;

                singletons.Add(new ODataSingleton
                {
                    Name = name,
                    EntityTypeName = entityTypeName,
                    EntityType = entityTypes.FirstOrDefault(et => et.Name == entityTypeName)!
                });
            }
        }

        return singletons;
    }

    private static List<ODataFunction> ParseActionImports(XElement entityContainer, XNamespace edmNamespace, string schemaNamespace, IReadOnlyList<XElement> allSchemas)
    {
        var functions = new List<ODataFunction>();

        // Parse ActionImport elements (V4)
        foreach (var actionImport in entityContainer.Elements(edmNamespace + "ActionImport"))
        {
            var name = actionImport.Attribute("Name")?.Value;
            var action = actionImport.Attribute("Action")?.Value;

            if (string.IsNullOrEmpty(name))
                continue;

            var actionName = action?.Contains('.') == true
                ? action.Split('.').Last()
                : action;

            // Determine preferred schema namespace from the fully-qualified action reference
            var actionRefNs = action?.Contains('.') == true
                ? string.Join(".", action.Split('.').SkipLast(1))
                : null;

            var function = new ODataFunction
            {
                Name = name,
                HttpMethod = "POST" // Actions in V4 are always POST
            };


            if (!string.IsNullOrEmpty(actionName))
            {
                // Search preferred schema first (namespace derived from fully-qualified reference), then fall back to all schemas
                XElement? actionElement = null;
                if (actionRefNs != null)
                {
                    var targetSchema = allSchemas.FirstOrDefault(s => s.Attribute("Namespace")?.Value == actionRefNs);
                    actionElement = targetSchema?.Elements(edmNamespace + "Action")
                        .FirstOrDefault(a => a.Attribute("Name")?.Value == actionName);
                }
                actionElement ??= allSchemas
                    .SelectMany(s => s.Elements(edmNamespace + "Action"))
                    .FirstOrDefault(a => a.Attribute("Name")?.Value == actionName);

                if (actionElement != null)
                {
                    // Parse return type
                    var returnTypeElement = actionElement.Element(edmNamespace + "ReturnType");
                    if (returnTypeElement != null)
                    {
                        function.ReturnType = returnTypeElement.Attribute("Type")?.Value;
                    }

                    // Parse parameters
                    foreach (var param in actionElement.Elements(edmNamespace + "Parameter"))
                    {
                        var paramName = param.Attribute("Name")?.Value;
                        var paramType = param.Attribute("Type")?.Value;

                        // Skip the binding parameter (first parameter with Type matching entity)
                        var isBound = actionElement.Attribute("IsBound")?.Value?.ToLower() == "true";
                        if (isBound && function.Parameters.Count == 0)
                            continue;

                        if (!string.IsNullOrEmpty(paramName) && !string.IsNullOrEmpty(paramType))
                        {
                            var parameter = new ODataFunctionParameter
                            {
                                Name = paramName,
                                DataType = paramType
                            };

                            if (int.TryParse(param.Attribute("MaxLength")?.Value, out int maxLength))
                                parameter.MaxLength = maxLength;

                            function.Parameters.Add(parameter);
                        }
                    }
                }
            }

            functions.Add(function);
        }

        // Parse FunctionImport elements (V4)
        foreach (var functionImport in entityContainer.Elements(edmNamespace + "FunctionImport"))
        {
            var name = functionImport.Attribute("Name")?.Value;
            var functionRef = functionImport.Attribute("Function")?.Value;

            if (string.IsNullOrEmpty(name))
                continue;

            var functionName = functionRef?.Contains('.') == true
                ? functionRef.Split('.').Last()
                : functionRef;

            var function = new ODataFunction
            {
                Name = name,
                HttpMethod = "GET" // Functions in V4 are always GET
            };

            // Find the Function element to get parameters and return type
            if (!string.IsNullOrEmpty(functionName))
            {
                // Determine the preferred schema namespace from the fully-qualified function reference
                var functionRefNs = functionRef?.Contains('.') == true
                    ? string.Join(".", functionRef.Split('.').SkipLast(1))
                    : null;

                // Search preferred schema first, then fall back to all schemas
                XElement? functionElement = null;
                if (functionRefNs != null)
                {
                    var targetSchema = allSchemas.FirstOrDefault(s => s.Attribute("Namespace")?.Value == functionRefNs);
                    functionElement = targetSchema?.Elements(edmNamespace + "Function")
                        .FirstOrDefault(f => f.Attribute("Name")?.Value == functionName);
                }
                functionElement ??= allSchemas
                    .SelectMany(s => s.Elements(edmNamespace + "Function"))
                    .FirstOrDefault(f => f.Attribute("Name")?.Value == functionName);

                if (functionElement != null)
                {
                    // Parse return type
                    var returnTypeElement = functionElement.Element(edmNamespace + "ReturnType");
                    if (returnTypeElement != null)
                    {
                        function.ReturnType = returnTypeElement.Attribute("Type")?.Value;
                    }

                    // Parse parameters
                    var isBoundFunc = functionElement.Attribute("IsBound")?.Value?.ToLower() == "true";
                    foreach (var param in functionElement.Elements(edmNamespace + "Parameter"))
                    {
                        var paramName = param.Attribute("Name")?.Value;
                        var paramType = param.Attribute("Type")?.Value;

                        // Skip the binding parameter (first parameter) for bound functions
                        if (isBoundFunc && function.Parameters.Count == 0)
                            continue;

                        if (!string.IsNullOrEmpty(paramName) && !string.IsNullOrEmpty(paramType))
                        {
                            var parameter = new ODataFunctionParameter
                            {
                                Name = paramName,
                                DataType = paramType
                            };

                            if (int.TryParse(param.Attribute("MaxLength")?.Value, out int maxLength))
                                parameter.MaxLength = maxLength;

                            function.Parameters.Add(parameter);
                        }
                    }
                }
            }

            functions.Add(function);
        }

        return functions;
    }

    private static void MarkEnumProperties(IReadOnlyList<ODataSchema> odataSchemas)
    {
        var enumTypeNames = new HashSet<string>(
            odataSchemas.SelectMany(s => s.EnumTypes).Select(e => e.Name)
        );

        var allEntityTypes = odataSchemas.SelectMany(s => s.EntityTypes);
        foreach (var entityType in allEntityTypes)
        {
            foreach (var property in entityType.Properties.Where(p => !p.DataType.StartsWith("Edm.")))
            {
                if (enumTypeNames.Contains(StripNamespace(property.DataType)))
                    property.IsEnumType = true;
            }
        }
    }
}
