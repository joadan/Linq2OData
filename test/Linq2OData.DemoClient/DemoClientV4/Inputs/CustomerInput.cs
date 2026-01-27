
using Linq2OData.Client;

namespace DemoClientV4.ODataDemo;

public partial class CustomerInput : ODataInputBase
{
    public decimal? TotalExpense 
	{
		get => GetValue<decimal?>("TotalExpense");
		set => SetValue("TotalExpense", value);
	}


}