namespace Linq2OData.Core;

[AttributeUsage(AttributeTargets.Enum)]
public class ODataEnumAttribute(string odataNamespace) : Attribute
{
    public string Namespace => odataNamespace;
}
