




using Linq2OData.Client;

namespace DemoClientV4.ODataDemo;

public partial class Advertisement 
{

    [ODataMember("ID")]
    public Guid ID { get; set; }

    [ODataMember("Name")]
    public string Name { get; set; }

    [ODataMember("AirDate")]
    public DateTimeOffset AirDate { get; set; }




    [ODataMember("FeaturedProduct")]
    public FeaturedProduct? FeaturedProduct { get; set; }

}

