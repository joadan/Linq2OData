namespace Linq2OData.Core.Metadata;

public class ODataMetadata
{
    public ODataVersion ODataVersion { get; set; }
    public string Namespace { get; set; } = string.Empty;
    public List<ODataEntitySet> EntitySets { get; set; } = [];
    public List<ODataEntityType> EntityTypes { get; set; } = [];
    public List<ODataEnumType> EnumTypes { get; set; } = [];
    public List<ODataFunction> Functions { get; set; } = [];

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

public class ODataEntitySet
{

    public required string Name { get; set; }
    public required string EntityTypeName { get; set; }
    public required ODataEntityType EntityType { get; set; }

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


}

public class ODataNavigation
{
    public required string Name { get; set; }

    public required string ToEntity { get; set; }

    public ODataNavigationType NavigationType { get; set; }

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

