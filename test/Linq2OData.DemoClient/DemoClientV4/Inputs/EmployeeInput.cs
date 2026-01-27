
using Linq2OData.Client;

namespace DemoClientV4.ODataDemo;

public partial class EmployeeInput : ODataInputBase
{
    public long? EmployeeID 
	{
		get => GetValue<long?>("EmployeeID");
		set => SetValue("EmployeeID", value);
	}
    public DateTimeOffset? HireDate 
	{
		get => GetValue<DateTimeOffset?>("HireDate");
		set => SetValue("HireDate", value);
	}
    public object? Salary 
	{
		get => GetValue<object?>("Salary");
		set => SetValue("Salary", value);
	}


}