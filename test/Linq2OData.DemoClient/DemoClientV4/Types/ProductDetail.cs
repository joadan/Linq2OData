




using Linq2OData.Client;

namespace DemoClientV4.ODataDemo;

public partial class ProductDetail
{

    [ODataMember("ProductID")]
    public int ProductID { get; set; }

    [ODataMember("Details")]
    public string Details { get; set; }




    [ODataMember("Product")]
    public Product? Product { get; set; }

}

