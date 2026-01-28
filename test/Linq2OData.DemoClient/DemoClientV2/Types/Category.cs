




using Linq2OData.Client;

namespace DemoClientV2.ODataDemo;

public partial class Category 
{

    [ODataMember("ID")]
    public int ID { get; set; }

    [ODataMember("Name")]
    public string Name { get; set; }




    [ODataMember("Products")]
    public List<Product>? Products { get; set; }

}

