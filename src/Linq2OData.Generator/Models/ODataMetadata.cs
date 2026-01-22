using System.Data;

namespace Linq2OData.Generator.Models;

public class ODataMetadata
{
    public ODataVersion ODataVersion { get; set; }
    public string Namespace { get; set; } = string.Empty;
    public List<ODataEntitySet> EntitySets { get; set; } = [];
    public List<ODataEntityType> EntityTypes { get; set; } = [];
    public List<ODataFunction> Functions { get; set; } = [];


    public string EndpointName => $"{Namespace}Endpoint";
    public string NamespaceEndpointName => $"{Namespace}.{EndpointName}";
}
public enum ODataVersion
{
    V2,
    V4
}

public class ODataEntitySet
{

    public required string Name { get; set; }
    public required string EntityType { get; set; }


    public string CSharpReturnType => $"ODataQuery<List<{EntityType}>>";
    public string CSharpMethodName => $"Get{Name}";

}
public class ODataEntityType
{
    public required string Name { get; set; }
    public string? Label { get; set; }

    public List<string> Keys { get; set; } = [];

    public List<ODataProperty> Properties { get; set; } = [];
    public List<ODataNavigation> Navigations { get; set; } = [];

}

public class ODataProperty
{
    public required string Name { get; set; }
    public bool Nullable { get; set; } = true;

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


    public string CSharpType
    {
        get
        {
            if (!DataType.StartsWith("Edm.")) {
                return DataType;
            }


            var csharpType = DataType switch
            {
                "Edm.String" => "string",
                "Edm.Int32" => "int",
                "Edm.Int64" => "long",
                "Edm.Boolean" => "bool",
                "Edm.DateTime" => "DateTime",
                "Edm.DateTimeOffset" => "DateTimeOffset",
                "Edm.Decimal" => "decimal",
                "Edm.Double" => "double",
                "Edm.Guid" => "Guid",
                "Edm.Time" => "TimeSpan",
                _ => "object"
            };


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

    public string CSharpProperty
    {
        get
        {

            if(NavigationType == ODataNavigationType.Many)
            {
                return $"List<{ToEntity}>?";
            }
          
                return ToEntity + "?";
          

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
