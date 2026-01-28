




using Linq2OData.Client;

namespace DemoClientV4.ODataDemo;

public partial class Person 
{

    [ODataMember("ID")]
    public int ID { get; set; }

    [ODataMember("Name")]
    public string Name { get; set; }




    [ODataMember("PersonDetail")]
    public PersonDetail? PersonDetail { get; set; }

}

