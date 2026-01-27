




using Linq2OData.Client;

namespace DemoClientV4.ODataDemo;

public partial class FeaturedProduct
{




    [ODataMember("Advertisement")]
    public Advertisement? Advertisement { get; set; }

}

