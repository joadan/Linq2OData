namespace Linq2OData.Core;

public class ODataEntitySetAttribute(string entityPath) :Attribute {

    public string EntityPath => entityPath;

}




