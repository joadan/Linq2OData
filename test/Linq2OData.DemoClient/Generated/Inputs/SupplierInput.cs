
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
    public ODataDemo.AddressInput? Address 
	{
		get => GetValue<ODataDemo.AddressInput?>("Address");
		set => SetValue("Address", value);
	}
    public int? Concurrency 
	{
		get => GetValue<int?>("Concurrency");
		set => SetValue("Concurrency", value);
	}

    public List<ProductInput>? Products 
	{
		get => GetValue<List<ProductInput>?>("Products");
		set => SetValue("Products", value);
	}

}