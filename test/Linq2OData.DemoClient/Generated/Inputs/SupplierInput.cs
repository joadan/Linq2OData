
using Linq2OData.Client;

namespace GeneratedClient.ODataDemo;

public partial class SupplierInput : ODataInputBase
{
    public int? ID 
	{
		get => GetValue<int?>("ID");
		set => SetValue("ID", value);
	}
    public string? Name 
	{
		get => GetValue<string?>("Name");
		set => SetValue("Name", value);
	}
    public ODataDemo.Address? Address 
	{
		get => GetValue<ODataDemo.Address?>("Address");
		set => SetValue("Address", value);
	}
    public int? Concurrency 
	{
		get => GetValue<int?>("Concurrency");
		set => SetValue("Concurrency", value);
	}

}