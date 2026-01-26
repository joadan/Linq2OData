using Linq2OData.Generator.Models;

using System.Xml.Linq;


namespace Linq2OData.Generator.Metadata;

internal static class MetadataParserVersion2
{
    internal static ODataMetadata Parse(XDocument doc)
    {
        
        var metadata = new ODataMetadata
        {
            ODataVersion = ODataVersion.V2
        };

        // Define namespaces
        XNamespace edmx = "http://schemas.microsoft.com/ado/2007/06/edmx";
        XNamespace edm = "http://schemas.microsoft.com/ado/2008/09/edm";
        XNamespace edm2007 = "http://schemas.microsoft.com/ado/2007/05/edm";
        XNamespace sap = "http://www.sap.com/Protocols/SAPData";
        XNamespace m = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

        // Get the schema element (try both EDM namespaces for V2 compatibility)
        var schema = doc.Descendants(edm + "Schema").FirstOrDefault()
                     ?? doc.Descendants(edm2007 + "Schema").FirstOrDefault();

        if (schema == null)
            return metadata;

        var currentEdmNamespace = schema.Name.Namespace;
        metadata.Namespace = schema.Attribute("Namespace")?.Value ?? string.Empty;

        // Parse EntityTypes
        metadata.EntityTypes = ParseEntityTypes(schema, currentEdmNamespace, sap);

        //Add ComplexTypes as EntityTypes
        metadata.EntityTypes.AddRange(ParseComplexTypes(schema, currentEdmNamespace, sap));

     

        // Parse EntityContainer to get EntitySets and Functions
        var entityContainer = schema.Descendants(currentEdmNamespace + "EntityContainer").FirstOrDefault();
        if (entityContainer != null)
        {
            metadata.EntitySets = ParseEntitySets(entityContainer, currentEdmNamespace, metadata.Namespace, metadata.EntityTypes);
            metadata.Functions = ParseFunctionImports(entityContainer, currentEdmNamespace, m, metadata.Namespace);
        }

        // Parse Associations to determine navigation cardinality
        ParseAssociations(schema, currentEdmNamespace, metadata.EntityTypes);

        return metadata;
    }

    private static List<ODataEntityType> ParseComplexTypes(XElement schema, XNamespace edmNamespace, XNamespace sap)
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
                var property = ParseProperty(prop, sap);
                if (property != null)
                    entityType.Properties.Add(property);
            }

          
        }

        return results;
    }

   


    private static List<ODataEntityType> ParseEntityTypes(XElement schema, XNamespace edmNamespace, XNamespace sap)
    {
        var entityTypes = new List<ODataEntityType>();

        foreach (var entityType in schema.Descendants(edmNamespace + "EntityType"))
        {
            var name = entityType.Attribute("Name")?.Value;
            if (string.IsNullOrEmpty(name))
                continue;

            var entity = new ODataEntityType
            {
                Name = name,
                Label = entityType.Attribute(sap + "label")?.Value
            };

            // Parse keys
            var keyElement = entityType.Element(edmNamespace + "Key");
            if (keyElement != null)
            {
                entity.Keys = keyElement.Descendants(edmNamespace + "PropertyRef")
                    .Select(pr => pr.Attribute("Name")?.Value)
                    .Where(k => !string.IsNullOrEmpty(k))
                    .ToList()!;
            }

            // Parse properties
            foreach (var prop in entityType.Elements(edmNamespace + "Property"))
            {
                var property = ParseProperty(prop, sap);
                if (property != null)
                    entity.Properties.Add(property);
            }

            // Parse navigation properties (will be completed after associations are parsed)
            foreach (var navProp in entityType.Elements(edmNamespace + "NavigationProperty"))
            {
                var navName = navProp.Attribute("Name")?.Value;
                var relationship = navProp.Attribute("Relationship")?.Value;
                var toRole = navProp.Attribute("ToRole")?.Value;
                var fromRole = navProp.Attribute("FromRole")?.Value;

                if (!string.IsNullOrEmpty(navName) && !string.IsNullOrEmpty(relationship))
                {
                    entity.Navigations.Add(new ODataNavigation
                    {
                        Name = navName,
                        ToEntity = string.Empty, // Will be filled when processing associations
                        NavigationType = ODataNavigationType.ZeroOrOne // Default, will be updated
                    });
                }
            }

            entityTypes.Add(entity);
        }

        return entityTypes;
    }

    private static ODataProperty? ParseProperty(XElement prop, XNamespace sap)
    {
        var name = prop.Attribute("Name")?.Value;
        var type = prop.Attribute("Type")?.Value;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
            return null;

        var property = new ODataProperty
        {
            Name = name,
            DataType = type,
            Nullable = prop.Attribute("Nullable")?.Value?.ToLower() != "false",
            Label = prop.Attribute(sap + "label")?.Value,
            Description = prop.Attribute(sap + "quickinfo")?.Value
        };

        // Parse MaxLength
        if (int.TryParse(prop.Attribute("MaxLength")?.Value, out int maxLength))
            property.MaxLength = maxLength;

        // Parse Precision and Scale
        if (int.TryParse(prop.Attribute("Precision")?.Value, out int precision))
            property.Precision = precision;

        if (int.TryParse(prop.Attribute("Scale")?.Value, out int scale))
            property.Scale = scale;

        // Parse SAP-specific attributes for creatable/updatable
        property.Creatable = prop.Attribute(sap + "creatable")?.Value?.ToLower() != "false";
        property.Updateble = prop.Attribute(sap + "updatable")?.Value?.ToLower() != "false";

        // Parse SAP-specific attributes for sortable/filterable
        property.Sortable = prop.Attribute(sap + "sortable")?.Value?.ToLower() != "false";
        property.Filterable = prop.Attribute(sap + "filterable")?.Value?.ToLower() != "false";

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

    private static List<ODataFunction> ParseFunctionImports(XElement entityContainer, XNamespace edmNamespace, XNamespace m, string schemaNamespace)
    {
        var functions = new List<ODataFunction>();

        foreach (var functionImport in entityContainer.Elements(edmNamespace + "FunctionImport"))
        {
            var name = functionImport.Attribute("Name")?.Value;
            if (string.IsNullOrEmpty(name))
                continue;

            var returnType = functionImport.Attribute("ReturnType")?.Value;
            var httpMethod = functionImport.Attribute(m + "HttpMethod")?.Value;

            var function = new ODataFunction
            {
                Name = name,
                ReturnType = returnType,
                HttpMethod = httpMethod
            };

            // Parse parameters
            foreach (var param in functionImport.Elements(edmNamespace + "Parameter"))
            {
                var paramName = param.Attribute("Name")?.Value;
                var paramType = param.Attribute("Type")?.Value;

                if (!string.IsNullOrEmpty(paramName) && !string.IsNullOrEmpty(paramType))
                {
                    var parameter = new ODataFunctionParameter
                    {
                        Name = paramName,
                        DataType = paramType,
                        Mode = param.Attribute("Mode")?.Value
                    };

                    if (int.TryParse(param.Attribute("MaxLength")?.Value, out int maxLength))
                        parameter.MaxLength = maxLength;

                    function.Parameters.Add(parameter);
                }
            }

            functions.Add(function);
        }

        return functions;
    }

    private static void ParseAssociations(XElement schema, XNamespace edmNamespace, List<ODataEntityType> entityTypes)
    {
        var associations = schema.Descendants(edmNamespace + "Association").ToList();

        foreach (var entityType in entityTypes)
        {
            foreach (var navigation in entityType.Navigations)
            {
                // Find the corresponding association
                var navProp = schema.Descendants(edmNamespace + "EntityType")
                    .FirstOrDefault(et => et.Attribute("Name")?.Value == entityType.Name)?
                    .Elements(edmNamespace + "NavigationProperty")
                    .FirstOrDefault(np => np.Attribute("Name")?.Value == navigation.Name);

                if (navProp == null)
                    continue;

                var relationshipName = navProp.Attribute("Relationship")?.Value;
                var toRole = navProp.Attribute("ToRole")?.Value;

                if (string.IsNullOrEmpty(relationshipName) || string.IsNullOrEmpty(toRole))
                    continue;

                // Find the association
                var association = associations.FirstOrDefault(a =>
                {
                    var assocName = a.Attribute("Name")?.Value;
                    return !string.IsNullOrEmpty(assocName) &&
                           (relationshipName.EndsWith(assocName) || relationshipName == assocName);
                });

                if (association == null)
                    continue;

                // Find the target end
                var targetEnd = association.Elements(edmNamespace + "End")
                    .FirstOrDefault(e => e.Attribute("Role")?.Value == toRole);

                if (targetEnd != null)
                {
                    var targetType = targetEnd.Attribute("Type")?.Value;
                    var multiplicity = targetEnd.Attribute("Multiplicity")?.Value;

                    if (!string.IsNullOrEmpty(targetType))
                    {
                        // Extract entity type name from full qualified name
                        navigation.ToEntity = targetType.Contains('.')
                            ? targetType.Split('.').Last()
                            : targetType;

                        // Determine navigation type based on multiplicity
                        navigation.NavigationType = multiplicity switch
                        {
                            "*" => ODataNavigationType.Many,
                            "1" => ODataNavigationType.One,
                            "0..1" => ODataNavigationType.ZeroOrOne,
                            _ => ODataNavigationType.ZeroOrOne
                        };
                    }
                }
            }
        }
    }
}
