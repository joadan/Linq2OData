using System.Xml.Linq;

namespace Linq2OData.Core.Metadata;

internal static class MetadataParserVersion4
{
    internal static ODataMetadata Parse(XDocument doc)
    {
        var metadata = new ODataMetadata
        {
            ODataVersion = ODataVersion.V4
        };

        // Define OData v4 namespaces
        XNamespace edmx = "http://docs.oasis-open.org/odata/ns/edmx";
        XNamespace edm = "http://docs.oasis-open.org/odata/ns/edm";

        // Get the schema element
        var schema = doc.Descendants(edm + "Schema").FirstOrDefault();

        if (schema == null)
            return metadata;

        metadata.Namespace = schema.Attribute("Namespace")?.Value ?? string.Empty;

        // Parse EnumTypes
        metadata.EnumTypes = ParseEnumTypes(schema, edm);

        // Parse EntityTypes
        metadata.EntityTypes = ParseEntityTypes(schema, edm);

        // Add ComplexTypes as EntityTypes
        metadata.EntityTypes.AddRange(ParseComplexTypes(schema, edm));

        // Parse EntityContainer to get EntitySets and Functions
        var entityContainer = schema.Descendants(edm + "EntityContainer").FirstOrDefault();
        if (entityContainer != null)
        {
            metadata.EntitySets = ParseEntitySets(entityContainer, edm, metadata.Namespace, metadata.EntityTypes);
            metadata.Functions = ParseActionImports(entityContainer, edm, metadata.Namespace);
        }

        metadata.SetEntityPaths();

        // Mark properties that use enum types
        MarkEnumProperties(metadata);

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

    private static List<ODataEntityType> ParseEntityTypes(XElement schema, XNamespace edmNamespace)
    {
        var entityTypes = new List<ODataEntityType>();

        foreach (var entityType in schema.Descendants(edmNamespace + "EntityType"))
        {
            var name = entityType.Attribute("Name")?.Value;
            var baseType = entityType.Attribute("BaseType")?.Value;
            if (string.IsNullOrEmpty(name))
                continue;

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

        var property = new ODataProperty
        {
            Name = name,
            DataType = type,
            Nullable = prop.Attribute("Nullable")?.Value?.ToLower() != "false"
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

    private static List<ODataEntitySet> ParseEntitySets(XElement entityContainer, XNamespace edmNamespace, string schemaNamespace, List<ODataEntityType> entityTypes)
    {
        var entitySets = new List<ODataEntitySet>();

        foreach (var entitySet in entityContainer.Elements(edmNamespace + "EntitySet"))
        {
            var name = entitySet.Attribute("Name")?.Value;
            var entityType = entitySet.Attribute("EntityType")?.Value;

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(entityType))
            {
                // Remove namespace prefix if present
                var entityTypeName = entityType.Contains('.')
                    ? entityType.Split('.').Last()
                    : entityType;

               
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

    

    private static List<ODataFunction> ParseActionImports(XElement entityContainer, XNamespace edmNamespace, string schemaNamespace)
    {
        var functions = new List<ODataFunction>();

        // Parse ActionImport elements (V4)
        foreach (var actionImport in entityContainer.Elements(edmNamespace + "ActionImport"))
        {
            var name = actionImport.Attribute("Name")?.Value;
            var action = actionImport.Attribute("Action")?.Value;

            if (string.IsNullOrEmpty(name))
                continue;

            // Find the corresponding Action definition in the schema
            var actionName = action?.Contains('.') == true
                ? action.Split('.').Last()
                : action;

            var function = new ODataFunction
            {
                Name = name,
                HttpMethod = "POST" // Actions in V4 are always POST
            };

            // Find the Action element to get parameters and return type
            if (!string.IsNullOrEmpty(actionName))
            {
                var schema = entityContainer.Parent;
                var actionElement = schema?.Elements(edmNamespace + "Action")
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
                var schema = entityContainer.Parent;
                var functionElement = schema?.Elements(edmNamespace + "Function")
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
                    foreach (var param in functionElement.Elements(edmNamespace + "Parameter"))
                    {
                        var paramName = param.Attribute("Name")?.Value;
                        var paramType = param.Attribute("Type")?.Value;

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

    private static void MarkEnumProperties(ODataMetadata metadata)
    {
        // Create a set of fully qualified enum type names for quick lookup
        var enumTypeNames = new HashSet<string>(
            metadata.EnumTypes.Select(e => $"{metadata.Namespace}.{e.Name}")
        );

        // Mark properties that reference enum types
        foreach (var entityType in metadata.EntityTypes)
        {
            foreach (var property in entityType.Properties)
            {
                // Check if it's a direct enum reference
                if (enumTypeNames.Contains(property.DataType))
                {
                    property.IsEnumType = true;
                }
                // Check if it's a collection of enums
                else if (property.DataType.StartsWith("Collection(") && property.DataType.EndsWith(")"))
                {
                    var innerType = property.DataType.Substring("Collection(".Length, 
                        property.DataType.Length - "Collection(".Length - 1);
                    if (enumTypeNames.Contains(innerType))
                    {
                        property.IsEnumType = true;
                    }
                }
            }
        }
    }
}
