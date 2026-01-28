




using Linq2OData.Client;

namespace DemoClientV4.ODataDemo;

public partial class Address 
{

    [ODataMember("Street")]
    public string Street { get; set; }

    [ODataMember("City")]
    public string City { get; set; }

    [ODataMember("State")]
    public string State { get; set; }

    [ODataMember("ZipCode")]
    public string ZipCode { get; set; }

    [ODataMember("Country")]
    public string Country { get; set; }




}

