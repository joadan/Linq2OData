




using Linq2OData.Client;

namespace GeneratedClient.ODataDemo;

public partial class Supplier
{

    [ODataMember("ID")]
    public int ID { get; set; }

    [ODataMember("Name")]
    public string Name { get; set; }

    [ODataMember("Address")]
    public ODataDemo.Address Address { get; set; }

    [ODataMember("Concurrency")]
    public int Concurrency { get; set; }




    [ODataMember("Products")]
    public List<Product>? Products { get; set; }

}

