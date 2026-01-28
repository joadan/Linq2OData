




using Linq2OData.Client;

namespace DemoClientV4.ODataDemo;

public partial class Customer : ODataDemo.Person
{

    [ODataMember("TotalExpense")]
    public decimal TotalExpense { get; set; }




}

