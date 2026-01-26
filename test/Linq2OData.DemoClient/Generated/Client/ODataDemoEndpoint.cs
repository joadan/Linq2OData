using Linq2OData.Client;

namespace GeneratedClient.ODataDemo;


public class ODataDemoEndpoint(ODataClient odataClient) 
{

    
    public ODataEntitySetQuery<Product> Products()
    { 
        return  new ODataEntitySetQuery<Product>(odataClient, "Products");
    }
    
    public ODataEntitySetQuery<Category> Categories()
    { 
        return  new ODataEntitySetQuery<Category>(odataClient, "Categories");
    }
    
    public ODataEntitySetQuery<Supplier> Suppliers()
    { 
        return  new ODataEntitySetQuery<Supplier>(odataClient, "Suppliers");
    }

    
    public ODataEntityQuery<Product> ProductsByKey(int id)
    { 
        return  new ODataEntityQuery<Product>(odataClient, "Products", $"ID={id}");
    }
    
    public ODataEntityQuery<Category> CategoriesByKey(int id)
    { 
        return  new ODataEntityQuery<Category>(odataClient, "Categories", $"ID={id}");
    }
    
    public ODataEntityQuery<Supplier> SuppliersByKey(int id)
    { 
        return  new ODataEntityQuery<Supplier>(odataClient, "Suppliers", $"ID={id}");
    }

    
    public async Task<bool> ProductsDeleteAsync(int id)
    { 
        return await odataClient.DeleteEntityAsync("Products", $"ID={id}");     
    }
    
    public async Task<bool> CategoriesDeleteAsync(int id)
    { 
        return await odataClient.DeleteEntityAsync("Categories", $"ID={id}");     
    }
    
    public async Task<bool> SuppliersDeleteAsync(int id)
    { 
        return await odataClient.DeleteEntityAsync("Suppliers", $"ID={id}");     
    }

    
    public async Task<Product> ProductsCreateAsync(ProductInput input)
    { 
        return await odataClient.CreateEntityAsync<Product>("Products", input);     
    }
    
    public async Task<Category> CategoriesCreateAsync(CategoryInput input)
    { 
        return await odataClient.CreateEntityAsync<Category>("Categories", input);     
    }
    
    public async Task<Supplier> SuppliersCreateAsync(SupplierInput input)
    { 
        return await odataClient.CreateEntityAsync<Supplier>("Suppliers", input);     
    }
 
     
    public async Task<bool> ProductsUpdateAsync(int id, ProductInput input)
    { 
        return await odataClient.UpdateEntityAsync("Products", $"ID={id}", input);     
    }
    
    public async Task<bool> CategoriesUpdateAsync(int id, CategoryInput input)
    { 
        return await odataClient.UpdateEntityAsync("Categories", $"ID={id}", input);     
    }
    
    public async Task<bool> SuppliersUpdateAsync(int id, SupplierInput input)
    { 
        return await odataClient.UpdateEntityAsync("Suppliers", $"ID={id}", input);     
    }
}