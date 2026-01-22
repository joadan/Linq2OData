




using Linq2OData.Client;

namespace DemoClient.ODataDemo;

public partial class Product
{

    [ODataMember("ID")]
    public int ID { get; set; }

    [ODataMember("Name")]
    public string Name { get; set; }

    [ODataMember("Description")]
    public string Description { get; set; }

    [ODataMember("ReleaseDate")]
    public DateTime ReleaseDate { get; set; }

    [ODataMember("DiscontinuedDate")]
    public DateTime? DiscontinuedDate { get; set; }

    [ODataMember("Rating")]
    public int Rating { get; set; }

    [ODataMember("Price")]
    public decimal Price { get; set; }




    [ODataMember("Category")]
    public Category? Category { get; set; }

    [ODataMember("Supplier")]
    public Supplier? Supplier { get; set; }

}

