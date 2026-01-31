

namespace Linq2OData.Generator.Models;

public class ODataMetadata
{
    public ODataVersion ODataVersion { get; set; }
    public string Namespace { get; set; } = string.Empty;
    public List<ODataEntitySet> EntitySets { get; set; } = [];
    public List<ODataEntityType> EntityTypes { get; set; } = [];
    public List<ODataEnumType> EnumTypes { get; set; } = [];
    public List<ODataFunction> Functions { get; set; } = [];

    internal string EndpointName => $"{Namespace}Endpoint";
    internal string NamespaceEndpointName => $"{Namespace}.{EndpointName}";

    public IEnumerable<ODataEntityType> GetDerivedTypes(string entityTypeName)
    {
        return EntityTypes.Where(et => et.BaseType == $"{Namespace}.{entityTypeName}");
    }

   

    internal void SetEntityPaths()
    {
        foreach (var entitySet in EntitySets)
        {
            entitySet.EntityType.EntityPath = entitySet.Name;
            SetEntityDerivedPaths(entitySet.EntityType);
        }
    }

    internal void SetEntityDerivedPaths(ODataEntityType entityType)
    {
        foreach (var derivedType in GetDerivedTypes(entityType.Name))
        {
            derivedType.EntityPath = $"{entityType.EntityPath}/{Namespace}.{derivedType.Name}";
            SetEntityDerivedPaths(derivedType);
        }
    }
}

public enum ODataVersion
{
    V2,
    V4
}

public class ODataEntitySet
{

    public required string Name { get; set; }
    public required string EntityTypeName { get; set; }
    public required ODataEntityType EntityType { get; set; }

    internal string CSharpReturnType => $"ODataEntitySetQuery<{EntityTypeName}>";
    internal string CSharpMethodName => $"{Name}";

    internal string CSharpKeyReturnType => $"ODataEntityQuery<{EntityTypeName}>";
    internal string CSharpKeyMethodName => $"{Name}ByKey";

    internal string CSharpDeleteMethodName => $"{Name}DeleteAsync";
    internal string CSharpCreateMethodName => $"{Name}CreateAsync";
    internal string CSharpUpdateMethodName => $"{Name}UpdateAsync";

}
public class ODataEntityType
{
    public required string Name { get; set; }
    public string? Label { get; set; }

    public string? BaseType { get; set; }

    public bool IsEntitySet => !string.IsNullOrWhiteSpace(EntityPath);

    public string? EntityPath { get; set; }

    public List<ODataProperty> Properties { get; set; } = [];
    public List<ODataNavigation> Navigations { get; set; } = [];

    public IEnumerable<ODataProperty> KeyProperties => Properties.Where(p => p.IsKey);


    internal string InputName => $"{Name}Input";


    internal string KeyArgumentString
    {
        get
        {
            if (!KeyProperties.Any()) { return string.Empty; }

            var keyArg = KeyProperties.Select(p =>
            {
                return $"{p.CSharpType} {Helpers.ToCamelCaseVariable(p.Name)}";
            });
            return string.Join(", ", keyArg);

        }
    }

    internal string KeyArgumentResultString
    {
        get
        {
            if (!KeyProperties.Any()) { return string.Empty; }

            var keyArg = KeyProperties.Select(p =>
            {
                return $"{p.KeyArgumentResult}";
            });

            return string.Join(",", keyArg);
        }
    }


}



public class ODataProperty
{
    public required string Name { get; set; }
    public bool Nullable { get; set; } = true;
    public bool IsKey { get; set; }
    public required string DataType { get; set; }

    public string? Description { get; set; }
    public string? Label { get; set; }

    public int? MaxLength { get; set; }

    public int? Precision { get; set; }
    public int? Scale { get; set; }

    public bool Creatable { get; set; }
    public bool Updateble { get; set; }

    public bool Sortable { get; set; }
    public bool Filterable { get; set; }

    public bool IsEnumType { get; set; } = false;

    internal string KeyArgumentResult
    {
        get
        {
            if (DataType.Equals("edm.string", StringComparison.CurrentCultureIgnoreCase))
            {
                return $"{Name}='{{{Helpers.ToCamelCaseVariable(Name)}}}'";
            }
            else
            {
                return $"{Name}={{{Helpers.ToCamelCaseVariable(Name)}}}";
            }

        }
    }



    internal string CSharpNameInput
    {
        get
        {
            // Handle Collection types
            if (DataType.StartsWith("Collection(") && DataType.EndsWith(")"))
            {
                var innerType = DataType.Substring("Collection(".Length, DataType.Length - "Collection(".Length - 1);
                
                // Check if it's an Edm type
                if (innerType.StartsWith("Edm."))
                {
                    // For Edm collections, use CSharpTypeRaw which already handles this
                    return CSharpTypeRaw + "?";
                }
                else
                {
                    // Custom type (complex type or enum)
                    var typeName = innerType.Contains('.') ? innerType.Split('.').Last() : innerType;
                    
                    // If the inner type is an enum, don't add "Input"
                    if (IsEnumType)
                    {
                        return $"List<{typeName}>?";
                    }
                    
                    // For complex types, add "Input"
                    return $"List<{typeName}Input>?";
                }
            }
            
            if (!DataType.StartsWith("Edm."))
            {
                // For enum types, use the enum type directly without appending "Input"
                if (IsEnumType)
                {
                    return CSharpTypeRaw + "?";
                }
                
                // For complex types, append "Input"
                return DataType + "Input?";
            }

            return CSharpTypeRaw + "?";

        }
    }


    internal string CSharpTypeRaw
    {
        get
        {
            // Handle Collection types
            if (DataType.StartsWith("Collection(") && DataType.EndsWith(")"))
            {
                var innerType = DataType.Substring("Collection(".Length, DataType.Length - "Collection(".Length - 1);
                
                // Check if it's an Edm type
                if (innerType.StartsWith("Edm."))
                {
                    var elementType = innerType switch
                    {
                        "Edm.String" => "string",
                        "Edm.Boolean" => "bool",
                        "Edm.Byte" => "byte",
                        "Edm.SByte" => "sbyte",
                        "Edm.Int16" => "short",
                        "Edm.Int32" => "int",
                        "Edm.Int64" => "long",
                        "Edm.Decimal" => "decimal",
                        "Edm.Single" => "float",
                        "Edm.Double" => "double",
                        "Edm.Guid" => "Guid",
                        "Edm.Binary" => "byte[]",
                        "Edm.DateTime" => "DateTime",
                        "Edm.Time" => "TimeSpan",
                        "Edm.Date" => "DateOnly",
                        "Edm.DateTimeOffset" => "DateTimeOffset",
                        "Edm.TimeOfDay" => "TimeSpan",
                        _ => "object"
                    };
                    return $"List<{elementType}>";
                }
                else
                {
                    // Custom type (complex type or enum) - strip namespace
                    var typeName = innerType.Contains('.') ? innerType.Split('.').Last() : innerType;
                    return $"List<{typeName}>";
                }
            }
            
            if (!DataType.StartsWith("Edm."))
            {
                // For custom types (complex types or enums), strip namespace prefix
                return DataType.Contains('.') ? DataType.Split('.').Last() : DataType;
            }


            var csharpType = DataType switch
            {
                // Common for v2 & v4
                "Edm.String" => "string",
                "Edm.Boolean" => "bool",
                "Edm.Byte" => "byte",
                "Edm.SByte" => "sbyte",
                "Edm.Int16" => "short",
                "Edm.Int32" => "int",
                "Edm.Int64" => "long",
                "Edm.Decimal" => "decimal",
                "Edm.Single" => "float",
                "Edm.Double" => "double",
                "Edm.Guid" => "Guid",
                "Edm.Binary" => "byte[]",

                // OData v2 only
                "Edm.DateTime" => "DateTime",
                "Edm.Time" => "TimeSpan",

                // OData v4 only
                "Edm.Date" => "DateOnly",          // or DateTime or DateOnly in .NET 6+
                "Edm.DateTimeOffset" => "DateTimeOffset",
                "Edm.TimeOfDay" => "TimeSpan",
                "Edm.GeographyPoint" => "object", // "Microsoft.Spatial.GeographyPoint", //This would make a dependancy to Microsoft.Spatial JsonConverter is needed..
                "Edm.GeometryPoint" => "object", // "Microsoft.Spatial.GeometryPoint",  //This would make a dependancy to Microsoft.Spatial 
                "Edm.Stream" => "object", //For now we use object not sure if we should handle this

                // Fallback
                _ => "object"
            };

            return csharpType;
        }

    }

    internal string CSharpType
    {
        get
        {

            var csharpType = CSharpTypeRaw;

            if (Nullable && csharpType != "string")
            {
                return csharpType + "?";
            }

            return csharpType;

        }
    }

}

public class ODataNavigation
{
    public required string Name { get; set; }

    public required string ToEntity { get; set; }

    public ODataNavigationType NavigationType { get; set; }

    internal string CSharpProperty
    {
        get
        {
            if (NavigationType == ODataNavigationType.Many)
            {
                return $"List<{ToEntity}>?";
            }
            return ToEntity + "?";
        }
    }

    internal string CSharpPropertyInput
    {
        get
        {
            if (NavigationType == ODataNavigationType.Many)
            {
                return $"List<{ToEntity}Input>?";
            }
            return ToEntity + "Input" + "?";
        }
    }
}
public class ODataFunction
{
    public required string Name { get; set; }
    public string? ReturnType { get; set; }
    public string? HttpMethod { get; set; }
    public List<ODataFunctionParameter> Parameters { get; set; } = [];
}

public class ODataFunctionParameter
{
    public required string Name { get; set; }
    public required string DataType { get; set; }
    public int? MaxLength { get; set; }
    public string? Mode { get; set; }
}

public enum ODataNavigationType
{
    ZeroOrOne,
    One,
    Many
}

public class ODataEnumType
{
    public required string Name { get; set; }
    public List<ODataEnumMember> Members { get; set; } = [];
}

public class ODataEnumMember
{
    public required string Name { get; set; }
    public required int Value { get; set; }
}

