
using Linq2OData.Client;

namespace GeneratedClient.ODataDemo;

public partial class AddressInput : ODataInputBase
{
    public string? Street 
	{
		get => GetValue<string?>("Street");
		set => SetValue("Street", value);
	}
    public string? City 
	{
		get => GetValue<string?>("City");
		set => SetValue("City", value);
	}
    public string? State 
	{
		get => GetValue<string?>("State");
		set => SetValue("State", value);
	}
    public string? ZipCode 
	{
		get => GetValue<string?>("ZipCode");
		set => SetValue("ZipCode", value);
	}
    public string? Country 
	{
		get => GetValue<string?>("Country");
		set => SetValue("Country", value);
	}

}