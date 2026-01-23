using Linq2OData.Client;

namespace GeneratedClient.ODataDemo;


public class ODataDemoEndpoint(ODataClient odataClient) 
{

    
    public ODataQuery<List<Product>> Products()
    { 
    return  new ODataQuery<List<Product>>(odataClient, "Products");
    }
    
    public ODataQuery<List<Category>> Categories()
    { 
    return  new ODataQuery<List<Category>>(odataClient, "Categories");
    }
    
    public ODataQuery<List<Supplier>> Suppliers()
    { 
    return  new ODataQuery<List<Supplier>>(odataClient, "Suppliers");
    }

    
    public ODataQuery<Product> ProductsByKey(int id)
    { 
    return  new ODataQuery<Product>(odataClient, "Products", $"ID={id}");
    }
    
    public ODataQuery<Category> CategoriesByKey(int id)
    { 
    return  new ODataQuery<Category>(odataClient, "Categories", $"ID={id}");
    }
    
    public ODataQuery<Supplier> SuppliersByKey(int id)
    { 
    return  new ODataQuery<Supplier>(odataClient, "Suppliers", $"ID={id}");
    }

}