namespace Linq2OData.Core;

public class ODataEntitySetAttribute(string entityPath) : ODataEntityAttribute
{
    public string EntityPath => entityPath;
}




