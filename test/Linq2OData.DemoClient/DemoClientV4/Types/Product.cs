




using Linq2OData.Client;

namespace DemoClientV4.ODataDemo;

public partial class Product 
{

    [ODataMember("ID")]
    public int ID { get; set; }

    [ODataMember("Name")]
    public string Name { get; set; }

    [ODataMember("Description")]
    public string Description { get; set; }

    [ODataMember("ReleaseDate")]
    public DateTimeOffset ReleaseDate { get; set; }

    [ODataMember("DiscontinuedDate")]
    public DateTimeOffset? DiscontinuedDate { get; set; }

    [ODataMember("Rating")]
    public short Rating { get; set; }

    [ODataMember("Price")]
    public double Price { get; set; }




    [ODataMember("Categories")]
    public List<Category>? Categories { get; set; }

    [ODataMember("Supplier")]
    public Supplier? Supplier { get; set; }

    [ODataMember("ProductDetail")]
    public ProductDetail? ProductDetail { get; set; }

}

