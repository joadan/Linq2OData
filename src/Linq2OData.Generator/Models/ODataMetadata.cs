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
    public required string EntityTypeName { get; set; }
    public required ODataEntityType EntityType { get; set; }

    private bool EntityTypeHasKeys => EntityType?.KeyProperties.Count() > 0;


    public string CSharpReturnType => $"ODataEntitySetQuery<{EntityTypeName}>";
    public string CSharpMethodName => $"{Name}";

    public string CSharpKeyReturnType => $"ODataEntityQuery<{EntityTypeName}>";
    public string CSharpKeyMethodName => $"{Name}ByKey";

    public string CSharpDeleteMethodName => $"{Name}DeleteAsync";
    public string CSharpCreateMethodName => $"{Name}CreateAsync";
    public string CSharpUpdateMethodName => $"{Name}UpdateAsync";

}
public class ODataEntityType
{
    public required string Name { get; set; }
    public string? Label { get; set; }

    //public List<string> Keys { get; set; } = [];

    public List<ODataProperty> Properties { get; set; } = [];
    public List<ODataNavigation> Navigations { get; set; } = [];

    public IEnumerable<ODataProperty> KeyProperties => Properties.Where(p => p.IsKey);

    public string InputName => $"{Name}Input";

    public string KeyArgumentString
    {
        get
        {
            if (KeyProperties.Count() == 0) { return string.Empty; }

            var keyArg = KeyProperties.Select(p =>
            {
                return $"{p.CSharpType} {ToCamelCaseVariable(p.Name)}";
            });
            return string.Join(", ", keyArg);

        }
    }

    public string KeyArgumentResultString
    {
        get
        {
            if (KeyProperties.Count() == 0) { return string.Empty; }

            var keyArg = KeyProperties.Select(p =>
            {
                return $"{p.Name}={{{ToCamelCaseVariable(p.Name)}}}";
            });

            return string.Join(",", keyArg);
        }
    }

    public static string ToCamelCaseVariable(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        int len = input.Length;
        int i = 0;

        // Find consecutive uppercase letters at the start
        while (i < len && char.IsUpper(input[i]))
            i++;

        // If first char is the only uppercase, just lowercase it
        if (i == 1)
            return char.ToLower(input[0]) + input.Substring(1);

        // If the whole string is uppercase (like "ID"), lowercase everything
        if (i == len)
            return input.ToLower();

        // Lowercase all leading uppercase letters except the last if next char is lowercase
        // Handles cases like "XMLHttpRequest" -> "xmlHttpRequest"
        int endOfAcronym = i;
        if (i > 1 && i < len && char.IsLower(input[i]))
            endOfAcronym = i - 1;

        return input.Substring(0, endOfAcronym).ToLower() + input.Substring(endOfAcronym);
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


    public string CSharpNameInput
    {
        get
        {
            if (!DataType.StartsWith("Edm."))
            {
                return DataType + "Input?";
            }

            return CSharpTypeRaw + "?";

        }
    }


    public string CSharpTypeRaw
    {
        get
        {
            if (!DataType.StartsWith("Edm."))
            {
                return DataType;
            }


            var csharpType = DataType switch
            {
                "Edm.String" => "string",
                "Edm.Int16" => "short",
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

            return csharpType;
        }

    }

    public string CSharpType
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

    public string CSharpProperty
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

    public string CSharpPropertyInput
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

