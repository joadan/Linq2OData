# Projection Support in Linq2OData

This document provides comprehensive information about using projections (the `.Select()` method) to retrieve specific fields from your OData queries.

## Overview

Projections allow you to specify exactly which fields you want to retrieve from your OData service, rather than fetching entire entities. This reduces bandwidth usage, improves performance, and makes your application more efficient.

```csharp
// Without projection - retrieves ALL fields
var products = await client.Query<Product>().ExecuteAsync();

// With projection - retrieves only Name and Price
var products = await client
    .Query<Product>()
    .Select(p => new { p.Name, p.Price })
    .ExecuteAsync();
```

## ⚠️ Important: Server vs Client Evaluation

**`.Select()` only sends `$select` and `$expand` to the server.** Any other operations within the Select expression (filtering, ordering, computations) are evaluated **client-side** after data is retrieved.

### What Gets Sent to the Server

- ✅ Property selection: `.Select(p => new { p.Name, p.Price })`
- ✅ Navigation expansion: `.Select(p => new { p.Category })`
- ✅ Nested property access: `.Select(p => new { CategoryName = p.Category.Name })`

### What Does NOT Get Sent to the Server

- ❌ Filtering within Select: `.Select(p => new { p.Name }).Where(x => x.Name.Contains("A"))`
- ❌ Ordering within Select: `.Select(p => new { p.Name }).OrderBy(x => x.Name)`
- ❌ Computed properties: `.Select(p => new { FullName = p.FirstName + " " + p.LastName })`
- ❌ Mathematical operations: `.Select(p => new { DiscountPrice = p.Price * 0.9m })`

### Best Practice: Filter and Order First

```csharp
// ✅ CORRECT - Server does heavy lifting
var products = await client
    .Query<Product>()
    .Filter(p => p.Price > 100)           // Server-side filter
    .Order(p => p.Name)                   // Server-side ordering
    .Top(50)                              // Server-side limit
    .Select(p => new { p.Name, p.Price }) // Server-side projection
    .ExecuteAsync();

// ❌ AVOID - Client does heavy lifting
var products = await client
    .Query<Product>()
    .Select(p => new { p.Name, p.Price }) // Gets all records
    .ExecuteAsync();
var filtered = products.Where(p => p.Price > 100);  // Client-side filter
```

## How It Works

When you use `.Select()`, Linq2OData analyzes your projection expression and generates the appropriate OData `$select` and `$expand` query parameters:

| Your LINQ Expression | Generated OData Query |
|---------------------|----------------------|
| `new { p.Name, p.Price }` | `$select=Name,Price` |
| `new { p.Name, p.Category }` | `$select=Name&$expand=Category` |
| `new { p.Name, CategoryName = p.Category.Name }` | `$select=Name&$expand=Category($select=Name)` |

## Basic Usage

### Simple Property Selection

Select specific scalar properties from your entity:

```csharp
var products = await client
    .Query<Product>()
    .Select(p => new { p.Name, p.Price, p.StockQuantity })
    .ExecuteAsync();

// Generates: $select=Name,Price,StockQuantity
```

### Single Property

You can select just one property:

```csharp
var productNames = await client
    .Query<Product>()
    .Select(p => new { p.Name })
    .ExecuteAsync();

// Generates: $select=Name
```

## Navigation Properties

### Including Full Navigation Objects

Include an entire related entity:

```csharp
var products = await client
    .Query<Product>()
    .Select(p => new 
    { 
        p.Name, 
        p.Price,
        p.Category  // Full Category object
    })
    .ExecuteAsync();

// OData v4: $select=Name,Price&$expand=Category
// OData v2: $select=Name,Price,Category&$expand=Category
```

### Selecting Properties from Navigation Objects

Select specific properties from related entities:

```csharp
var products = await client
    .Query<Product>()
    .Select(p => new 
    { 
        p.Name, 
        p.Price,
        CategoryName = p.Category.Name,  // Only the Name from Category
        CategoryCode = p.Category.Code
    })
    .ExecuteAsync();

// Generates: $select=Name,Price&$expand=Category($select=Name,Code)
```

## Deep Nesting

### Multi-Level Navigation

Access properties through multiple levels of navigation:

```csharp
var orders = await client
    .Query<Order>()
    .Select(o => new 
    { 
        o.OrderNumber,
        CustomerName = o.Customer.Name,
        CustomerCity = o.Customer.Address.City,
        CustomerCountry = o.Customer.Address.Country.Name
    })
    .ExecuteAsync();

// OData v4:
// $select=OrderNumber
// &$expand=Customer($select=Name;$expand=Address($select=City;$expand=Country($select=Name)))

// OData v2/v3:
// $select=OrderNumber,Customer
// &$expand=Customer/Address/Country
```

### Nested Navigation Without Properties

You can also project just the nested entity itself:

```csharp
var people = await client
    .Query<Person>()
    .Select(p => new 
    { 
        p.Name,
        PersonDetailPerson = p.PersonDetail.Person  // Full nested Person object
    })
    .ExecuteAsync();

// OData v4: $select=Name&$expand=PersonDetail($expand=Person)
// OData v2/v3: $select=Name,PersonDetail&$expand=PersonDetail/Person
```

## Complex Scenarios

### Mixed Projections

Combine simple properties, nested properties, and full objects:

```csharp
var suppliers = await client
    .Query<Supplier>()
    .Select(s => new 
    { 
        // Simple properties
        s.Name,
        s.Email,
        s.Phone,
        
        // Nested properties
        ContactFirstName = s.PrimaryContact.FirstName,
        ContactLastName = s.PrimaryContact.LastName,
        
        // Full object
        Address = s.Address,
        
        // Collection
        AllProducts = s.Products
    })
    .ExecuteAsync();

// Generates:
// $select=Name,Email,Phone
// &$expand=PrimaryContact($select=FirstName,LastName);Address;Products
```

### Multiple Properties from Same Navigation

Access multiple properties from the same navigation entity:

```csharp
var orders = await client
    .Query<Order>()
    .Select(o => new 
    { 
        o.OrderNumber,
        CustomerName = o.Customer.Name,
        CustomerEmail = o.Customer.Email,
        CustomerPhone = o.Customer.Phone
    })
    .ExecuteAsync();

// Generates: $select=OrderNumber&$expand=Customer($select=Name,Email,Phone)
```

## Use with Single Entity Retrieval

Projections work with the `.Get()` method for retrieving single entities by key:

```csharp
var product = await client
    .Get<Product>(p => p.ID = 123)
    .Select(p => new 
    { 
        p.Name, 
        p.Price,
        CategoryName = p.Category.Name
    })
    .ExecuteAsync();

// Generates: Products(ID=123)?$select=Name,Price&$expand=Category($select=Name)
```

## Projecting to Custom Types

While anonymous types are convenient for quick queries, you can also project to your own DTOs (Data Transfer Objects) or view models. This is especially useful for:
- API responses
- Serialization
- Type reusability across multiple queries
- Better IntelliSense and refactoring support

### Basic DTO Projection

```csharp
// Define your DTO
public class ProductSummaryDto
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string CategoryName { get; set; }
}

// Project to the DTO
var products = await client
    .Query<Product>()
    .Filter(p => p.InStock)
    .Select(p => new ProductSummaryDto
    {
        Name = p.Name,
        Price = p.Price,
        CategoryName = p.Category.Name
    })
    .ExecuteAsync();
// Returns: List<ProductSummaryDto>
```

### DTO with Client-Side Computations

```csharp
public class ProductDetailDto
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPrice { get; set; }
    public string FullCategoryPath { get; set; }
}

var products = await client
    .Query<Product>()
    .Filter(p => p.Price > 50)
    .Select(p => new ProductDetailDto
    {
        Name = p.Name,                                    // ✅ Server sends
        Price = p.Price,                                  // ✅ Server sends
        DiscountPrice = p.Price * 0.85m,                  // ⚠️ Client computes
        FullCategoryPath = p.Category.Parent.Name + " > " + p.Category.Name  // ⚠️ Client computes
    })
    .ExecuteAsync();
// Server sends: Name, Price, Category.Parent.Name, Category.Name
// Client computes: DiscountPrice, FullCategoryPath
// Returns: List<ProductDetailDto>
```

### Benefits of Custom Types

```csharp
// ✅ Reusable across multiple queries
public class OrderSummaryDto
{
    public int OrderNumber { get; set; }
    public decimal Total { get; set; }
    public string CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
}

// Use in different queries
var recentOrders = await client
    .Query<Order>()
    .Filter(o => o.OrderDate > DateTime.Now.AddDays(-7))
    .Select(o => new OrderSummaryDto { /* ... */ })
    .ExecuteAsync();

var highValueOrders = await client
    .Query<Order>()
    .Filter(o => o.Total > 1000)
    .Select(o => new OrderSummaryDto { /* ... */ })
    .ExecuteAsync();

// ✅ Perfect for API responses
[HttpGet("products")]
public async Task<ActionResult<List<ProductSummaryDto>>> GetProducts()
{
    return await client
        .Query<Product>()
        .Select(p => new ProductSummaryDto { /* ... */ })
        .ExecuteAsync();
}
```

## Combining with Other Query Operations

Projections work seamlessly with filtering, ordering, and pagination:

```csharp
var products = await client
    .Query<Product>()
    .Filter(p => p.Price > 100)           // Server-side filter
    .Order(p => p.Name)                   // Server-side ordering
    .Top(20)                              // Server-side limit
    .Skip(10)                             // Server-side pagination
    .Select(p => new { p.Name, p.Price, CategoryName = p.Category.Name })
    .ExecuteAsync();

// Generates:
// Products?$filter=Price gt 100
//         &$orderby=Name
//         &$top=20
//         &$skip=10
//         &$select=Name,Price
//         &$expand=Category($select=Name)
```

## Null Handling

### Nullable Navigation Properties

Use the null-forgiving operator (`!`) for nullable navigation properties:

```csharp
var products = await client
    .Query<Product>()
    .Select(p => new 
    { 
        p.Name,
        CategoryName = p.Category!.Name  // ! indicates Category might be null
    })
    .ExecuteAsync();
```

The library correctly handles the null-forgiving operator and generates valid OData queries.

## OData Version Differences

Linq2OData automatically adapts to your OData version, but it's helpful to understand the differences:

### OData v4 Syntax

```
$select=Name,Price
$expand=Category($select=Name;$expand=Products)
```

- Uses nested parentheses
- `$select=` and `$expand=` prefixes in nested queries
- Semicolon (`;`) separates select and expand within parentheses

### OData v2/v3 Syntax

```
$select=Name,Price,Category
$expand=Category/Products
```

- Uses slash notation (`/`) for nesting
- No nested `$expand=` prefixes
- Complex/navigation properties appear in both `$select` and `$expand`

### The Library Handles This Automatically

You write the same LINQ code regardless of OData version:

```csharp
// Same code for both versions
.Select(p => new { p.Name, Category = p.Category.Supplier.Name })

// OData v4 generates:
// $select=Name&$expand=Category($expand=Supplier($select=Name))

// OData v2/v3 generates:
// $select=Name,Category&$expand=Category/Supplier
```

## Performance Benefits

### Bandwidth Reduction

**Without projection:**
```json
{
  "ID": 123,
  "Name": "Product A",
  "Description": "A very long description...",
  "Price": 99.99,
  "StockQuantity": 50,
  "CreatedDate": "2024-01-01T00:00:00Z",
  "ModifiedDate": "2024-01-15T10:30:00Z",
  "Category": { /* full object */ },
  "Supplier": { /* full object */ },
  "Reviews": [ /* array of reviews */ ],
  // ... many more fields
}
```

**With projection:**
```json
{
  "Name": "Product A",
  "Price": 99.99,
  "CategoryName": "Electronics"
}
```

### Query Performance

- **Smaller payloads** = faster network transfer
- **Less data processing** on the server
- **Reduced JSON parsing** on the client
- **Lower memory usage** in your application

### Example Impact

| Scenario | Without Projection | With Projection | Savings |
|----------|-------------------|-----------------|---------|
| 100 products, all fields | ~500 KB | ~50 KB | **90%** |
| 1000 orders with customers | ~5 MB | ~500 KB | **90%** |
| Single entity with deep navigation | ~100 KB | ~5 KB | **95%** |

*Note: Actual savings depend on your entity structure and selected fields.*

## Best Practices

### Filter and Order First

Apply server-side operations before projections to minimize data transfer:

```csharp
// ✅ OPTIMAL - Server does the heavy lifting
var products = await client
    .Query<Product>()
    .Filter(p => p.Price > 100)           // Server-side filter
    .Order(p => p.Name)                   // Server-side ordering
    .Top(20)                              // Server-side limit
    .Select(p => new { p.Name, p.Price }) // Server-side projection
    .ExecuteAsync();

// ⚠️ Less efficient - Fetches all fields, then projects client-side
var products = await client
    .Query<Product>()
    .Filter(p => p.Price > 100)
    .Order(p => p.Name)
    .Top(20)
    .ExecuteAsync();                      // Gets all fields
var projected = products.Select(p => new { p.Name, p.Price });  // Client-side projection
```

### Be Specific

Only select fields you actually need:

```csharp
// ✅ Good - Only needed fields
.Select(p => new { p.Name, p.Price })

// ❌ Bad - Unnecessary fields
.Select(p => new { p.Name, p.Price, p.Description, p.Category, p.Supplier })
```

### Use Navigation Properties Wisely

Consider whether you need the full object or just specific properties:

```csharp
// ✅ Good - Only need category name
.Select(p => new { p.Name, CategoryName = p.Category.Name })

// ⚠️ Potentially wasteful - Full Category object
.Select(p => new { p.Name, p.Category })
```

### Avoid Over-Nesting

Very deep nesting can be hard to maintain:

```csharp
// ⚠️ Consider if you really need this level of nesting
CustomerCountryRegionProvinceCityName = o.Customer.Address.Country.Region.Province.City.Name

// ✅ Better - Flatten or reconsider your data model
```

## Limitations and Considerations

### 1. Anonymous Types vs Custom Types

Anonymous types are great for quick queries but have limitations:

```csharp
// ⚠️ Anonymous types - Not serializable, can't be returned from methods easily
var data = await client
    .Query<Product>()
    .Select(p => new { p.Name, p.Price })
    .ExecuteAsync();

// ❌ Won't work - Can't return anonymous types from API methods
public async Task<object> GetProducts()
{
    return await client.Query<Product>().Select(p => new { p.Name }).ExecuteAsync();
}

// ✅ Better - Use DTOs for API boundaries
public class ProductDto { public string Name { get; set; } }

public async Task<List<ProductDto>> GetProducts()
{
    return await client
        .Query<Product>()
        .Select(p => new ProductDto { Name = p.Name })
        .ExecuteAsync();
}
```

### 2. Computed Properties

**Important:** You can only project properties that exist in the OData model. Computed expressions are evaluated **client-side**.

#### When Client-Side Computations Are Useful

There are legitimate use cases for client-side computations in Select:
- **OData doesn't support the operation** (string concatenation, complex math)
- **Custom business logic** not available on the server
- **Quick prototyping** before implementing server-side support
- **Small datasets** where client-side computation is acceptable

The library automatically handles both server-side projection and client-side computation in a single `.Select()`:

```csharp
// ✅ Smart approach - Mix server projection with client computation
var people = await client
    .Query<Person>()
    .Filter(p => p.Age > 18)              // ✅ Server filters first
    .Top(100)                             // ✅ Limit dataset
    .Select(p => new 
    { 
        p.FirstName,                      // ✅ Server sends
        p.LastName,                       // ✅ Server sends
        p.Email,                          // ✅ Server sends
        FullName = p.FirstName + " " + p.LastName  // ⚠️ Client computes
    })
    .ExecuteAsync();
// Server sends: FirstName, LastName, Email (3 fields × 100 records)
// Client adds: FullName (computed from the fetched data)
// Result: Clean object with all 4 properties
```

#### Performance Consideration: Filter Before Computing

The library intelligently separates server-side projections from client-side computations within a single `.Select()`:

```csharp
// ⚠️ Less efficient - No server-side filtering
var people = await client
    .Query<Person>()
    .Select(p => new 
    { 
        FullName = p.FirstName + " " + p.LastName,  // Client-side
        Age = DateTime.Now.Year - p.BirthDate.Year  // Client-side
    })
    .ExecuteAsync();
// Fetches ALL people with ALL fields, computes everything client-side

// ✅ More efficient - Filter first, then project and compute
var people = await client
    .Query<Person>()
    .Filter(p => p.IsActive && p.Age > 18)  // ✅ Server filters first
    .Top(100)                                // ✅ Server limits
    .Select(p => new 
    { 
        p.FirstName,                         // ✅ Server sends
        p.LastName,                          // ✅ Server sends
        p.BirthDate,                         // ✅ Server sends
        FullName = p.FirstName + " " + p.LastName,  // ⚠️ Client computes
        Age = DateTime.Now.Year - p.BirthDate.Year  // ⚠️ Client computes
    })
    .ExecuteAsync();
// Server sends only: FirstName, LastName, BirthDate (3 fields × 100 records)
// Client computes: FullName, Age
// Result has all 5 properties in one object!

// ✅ BEST - If your OData service supports computed properties
var people = await client
    .Query<Person>()
    .Filter(p => p.IsActive && p.Age > 18)
    .Select(p => new { p.FullName, p.Age })  // Server-computed (if available)
    .ExecuteAsync();
```

#### Recommendation

When you need computed properties:

1. **Prefer server-side** if your OData service supports it
2. **Filter first** to reduce the dataset before client-side computation
3. **Project only needed fields** to minimize data transfer
4. **Understand the trade-off** between convenience and performance

```csharp
// Example decision tree:
// 1. Does the OData service support this computation? → Use server-side
// 2. Is the dataset small (<100 records)? → Client-side is fine
// 3. Large dataset? → Filter first, project minimal fields, then compute
```

### 3. Collection Navigation Properties

When projecting collection navigation properties, you get the entire collection:

```csharp
.Select(c => new { c.Name, c.Orders })  // All orders, can't filter here
```

To filter collections, use nested queries (if supported by your OData service):

```csharp
.Expand(c => c.Orders.Filter(o => o.Status == "Shipped"))  // If supported
```

## Troubleshooting

### Query Returns Empty Data

**Problem:** Properties in the result are null or missing.

**Solution:** Ensure property names match exactly (case-sensitive):

```csharp
// ❌ If OData has "ProductName" but you use:
.Select(p => new { p.Name })  // Won't match

// ✅ Use correct property name:
.Select(p => new { p.ProductName })
```

### Invalid OData Syntax Error

**Problem:** OData service rejects the query.

**Solution:** Check your OData version. The library generates version-appropriate syntax, but ensure you've configured the correct version:

```csharp
var client = new MyODataClient(httpClient, ODataVersion.V4);  // Specify version
```

### Performance Not Improving

**Problem:** Projections don't seem to help performance.

**Solution:** 
1. Verify the projection is being applied (check the URL)
2. Ensure your OData service supports `$select` and `$expand`
3. Check server-side logging to confirm smaller payloads

## Examples Repository

For more examples, see the test suites:

- **`test/Linq2OData.Tests/SelectProjectionTests.cs`** - Unit tests with many scenarios
- **`test/Linq2OData.Tests/GetProjectionIntegrationTests.cs`** - Integration tests
- **`test/Linq2OData.TestClients/Program.cs`** - Real-world usage examples

## Related Documentation

- [OData Version Support](ODataVersionSupport.md) - Version-specific features
- [Navigation Property Serialization](NavigationPropertySerialization.md) - How navigation properties are handled
- [Naming Convention](NamingConvention.md) - API design philosophy

## Need Help?

If you have questions or issues with projections:

1. Check the [GitHub Issues](https://github.com/joadan/Linq2OData/issues)
2. Review the test suites for examples
3. Open a new issue with a code sample

---

**Key Takeaway:** Projections are a powerful feature for optimizing your OData queries. Use `.Select()` to retrieve only what you need, and let Linq2OData handle the version-specific OData syntax automatically.
