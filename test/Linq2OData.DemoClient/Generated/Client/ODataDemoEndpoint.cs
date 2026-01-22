using Linq2OData.Client;

namespace GeneratedClient.ODataDemo;


public class ODataDemoEndpoint(ODataClient odataClient) 
{

    
    public ODataQuery<List<Product>> GetProducts()
    { 
    return  new ODataQuery<List<Product>>(odataClient, "Products");
    }
    
    public ODataQuery<List<Category>> GetCategories()
    { 
    return  new ODataQuery<List<Category>>(odataClient, "Categories");
    }
    
    public ODataQuery<List<Supplier>> GetSuppliers()
    { 
    return  new ODataQuery<List<Supplier>>(odataClient, "Suppliers");
    }

}