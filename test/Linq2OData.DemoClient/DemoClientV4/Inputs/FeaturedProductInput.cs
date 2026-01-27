
using Linq2OData.Client;

namespace DemoClientV4.ODataDemo;

public partial class FeaturedProductInput : ODataInputBase
{

    public AdvertisementInput? Advertisement 
	{
		get => GetValue<AdvertisementInput?>("Advertisement");
		set => SetValue("Advertisement", value);
	}

}