
using Linq2OData.Client;

namespace DemoClientV4.ODataDemo;

public partial class ProductDetailInput : ODataInputBase
{
    public int? ProductID 
	{
		get => GetValue<int?>("ProductID");
		set => SetValue("ProductID", value);
	}
    public string? Details 
	{
		get => GetValue<string?>("Details");
		set => SetValue("Details", value);
	}

    public ProductInput? Product 
	{
		get => GetValue<ProductInput?>("Product");
		set => SetValue("Product", value);
	}

}