
using Linq2OData.Client;

namespace DemoClientV4.ODataDemo;

public partial class PersonInput : ODataInputBase
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

    public PersonDetailInput? PersonDetail 
	{
		get => GetValue<PersonDetailInput?>("PersonDetail");
		set => SetValue("PersonDetail", value);
	}

}