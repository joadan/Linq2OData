
using Linq2OData.Client;

namespace DemoClientV2.ODataDemo;

public partial class CategoryInput : ODataInputBase
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

    public List<ProductInput>? Products 
	{
		get => GetValue<List<ProductInput>?>("Products");
		set => SetValue("Products", value);
	}

}