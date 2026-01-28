




using Linq2OData.Client;

namespace DemoClientV4.ODataDemo;

public partial class Employee : ODataDemo.Person
{

    [ODataMember("EmployeeID")]
    public long EmployeeID { get; set; }

    [ODataMember("HireDate")]
    public DateTimeOffset HireDate { get; set; }

    [ODataMember("Salary")]
    public object Salary { get; set; }




}

