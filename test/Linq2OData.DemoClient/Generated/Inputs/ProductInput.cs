
using Linq2OData.Client;

namespace GeneratedClient.ODataDemo;

public partial class ProductInput : ODataInputBase
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
    public string? Description 
	{
		get => GetValue<string?>("Description");
		set => SetValue("Description", value);
	}
    public DateTime? ReleaseDate 
	{
		get => GetValue<DateTime?>("ReleaseDate");
		set => SetValue("ReleaseDate", value);
	}
    public DateTime? DiscontinuedDate 
	{
		get => GetValue<DateTime?>("DiscontinuedDate");
		set => SetValue("DiscontinuedDate", value);
	}
    public int? Rating 
	{
		get => GetValue<int?>("Rating");
		set => SetValue("Rating", value);
	}
    public decimal? Price 
	{
		get => GetValue<decimal?>("Price");
		set => SetValue("Price", value);
	}

    public CategoryInput? Category 
	{
		get => GetValue<CategoryInput?>("Category");
		set => SetValue("Category", value);
	}
    public SupplierInput? Supplier 
	{
		get => GetValue<SupplierInput?>("Supplier");
		set => SetValue("Supplier", value);
	}

}