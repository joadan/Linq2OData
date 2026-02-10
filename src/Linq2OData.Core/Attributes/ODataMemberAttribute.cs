namespace Linq2OData.Core;

public class ODataMemberAttribute(string name, bool isComplex = false) :Attribute {

    public string Name => name;
    public bool IsComplex => isComplex;
}




