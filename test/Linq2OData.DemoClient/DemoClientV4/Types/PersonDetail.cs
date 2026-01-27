




using Linq2OData.Client;

namespace DemoClientV4.ODataDemo;

public partial class PersonDetail
{

    [ODataMember("PersonID")]
    public int PersonID { get; set; }

    [ODataMember("Age")]
    public object Age { get; set; }

    [ODataMember("Gender")]
    public bool Gender { get; set; }

    [ODataMember("Phone")]
    public string Phone { get; set; }

    [ODataMember("Address")]
    public ODataDemo.Address? Address { get; set; }

    [ODataMember("Photo")]
    public object Photo { get; set; }




    [ODataMember("Person")]
    public Person? Person { get; set; }

}

