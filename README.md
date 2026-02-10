# Linq2OData

[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![NuGet](https://img.shields.io/badge/NuGet-Linq2OData-orange.svg)](https://www.nuget.org/packages/Linq2OData.Core/)

A modern, type-safe .NET library for building OData queries using LINQ expressions. Linq2OData provides an intuitive fluent API that works seamlessly with OData v2, v3, and v4 endpoints, automatically adapting query syntax to match your target OData version.

## 🌟 Features

- **🔷 Type-Safe LINQ Queries** - Write queries using strongly-typed C# expressions instead of error-prone strings
- **🔄 Multi-Version Support** - Seamlessly supports OData v2, v3, and v4 with automatic syntax adaptation
- **⚡ Fluent API** - Intuitive, chainable API following OData terminology
- **🔧 Client Generator** - Generate type-safe client code from OData metadata ($metadata)
- **🎯 Full OData Feature Set** - Complete support for `$filter`, `$expand`, `$orderby`, `$top`, `$skip`, `$count`, and `$select`
- **🔗 Complex Queries** - Support for nested expands, complex filters, and multi-level ordering
- **📦 Lightweight** - Minimal dependencies, built on .NET 10
- **🧪 Well-Tested** - Comprehensive test coverage with real-world scenarios

## 📦 Installation

Install via NuGet Package Manager:

```bash
dotnet add package Linq2OData.Core
```

Or use the Package Manager Console:

```powershell
Install-Package Linq2OData.Core
```

For client generation capabilities, also install:

```bash
dotnet add package Linq2OData.Generator
```

## 🚀 Quick Start

### Basic Setup

```csharp
using Linq2OData.Core;
using System.Net.Http;

// Initialize HttpClient
var httpClient = new HttpClient 
{ 
    BaseAddress = new Uri("https://services.odata.org/V4/OData/OData.svc/")
};

// Create OData client (specify version: V2, V3, or V4)
var client = new ODataClient(httpClient, ODataVersion.V4);
```

### Simple Query

```csharp
// Query products with filtering and ordering
var products = await client
    .Query<Product>()
    .Filter(p => p.Price > 10 && p.InStock)
    .Order(p => p.Name)
    .Top(20)
    .ExecuteAsync();

// Generates: /Products?$filter=(Price gt 10) and (InStock eq true)&$orderby=Name&$top=20
```

### Complex Query with Expands

```csharp
var suppliers = await client
    .Query<Supplier>()
    .Expand(s => s.Products)
        .ThenExpand(products => products.Select(p => p.Category))
    .Filter(s => s.Country == "USA" && s.Rating > 4)
    .Order(s => s.Name)
    .Top(50)
    .ExecuteAsync();

// OData v4 generates:
// /Suppliers?$expand=Products($expand=Category)&$filter=(Country eq 'USA') and (Rating gt 4)&$orderby=Name&$top=50

// OData v2/v3 generates:
// /Suppliers?$expand=Products/Category&$filter=(Country eq 'USA') and (Rating gt 4)&$orderby=Name&$top=50
```

## 📖 Core Concepts

### OData Version Support

Linq2OData automatically adapts query syntax based on your configured OData version:

```csharp
// OData v2
var clientV2 = new ODataClient(httpClient, ODataVersion.V2);

// OData v3
var clientV3 = new ODataClient(httpClient, ODataVersion.V3);

// OData v4
var clientV4 = new ODataClient(httpClient, ODataVersion.V4);
```

Key differences are handled automatically:
- **Expand syntax**: v2/v3 uses `/` while v4 uses nested `$expand`
- **Date handling**: v2/v3 uses `/Date()/ format, v4 uses ISO 8601
- **Data wrapping**: v2/v3 wraps collections in `results`, v4 doesn't

### Defining Entity Types

```csharp
[ODataEntitySet("Products")]
public class Product : IODataEntitySet
{
    public int ID { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool InStock { get; set; }
    public Category? Category { get; set; }
    public List<Tag>? Tags { get; set; }
    
    public string __Key => $"ID={ID}";
}
```

## 🎯 Feature Guide

### Filtering

Use strongly-typed LINQ expressions for filters:

```csharp
// Simple comparison
.Filter(p => p.Price > 100)
// Generates: $filter=(Price gt 100)

// String operations
.Filter(p => p.Name.Contains("Phone"))
// Generates: $filter=contains(Name,'Phone')

.Filter(p => p.Name.StartsWith("Apple"))
// Generates: $filter=startswith(Name,'Apple')

// Logical operators
.Filter(p => p.Price > 10 && p.Price < 100)
// Generates: $filter=(Price gt 10) and (Price lt 100)

.Filter(p => p.Category.Name == "Electronics" || p.Category.Name == "Computers")
// Generates: $filter=(Category/Name eq 'Electronics') or (Category/Name eq 'Computers')

// Date comparisons
.Filter(p => p.CreatedDate > DateTime.Now.AddMonths(-6))

// Null checks
.Filter(p => p.DiscontinuedDate == null)
// Generates: $filter=(DiscontinuedDate eq null)
```

### Expanding Navigation Properties

Expand related entities with nested support:

```csharp
// Simple expand
.Expand(s => s.Orders)
// OData v4: $expand=Orders
// OData v2/v3: $expand=Orders

// Nested expand (single property chain)
.Expand(o => o.Customer)
    .ThenExpand(c => c.Address)
    .ThenExpand(a => a.Country)
// OData v4: $expand=Customer($expand=Address($expand=Country))
// OData v2/v3: $expand=Customer/Address/Country

// Collection expand with Select()
.Expand(s => s.Products)
    .ThenExpand(products => products.Select(p => p.Category))
// OData v4: $expand=Products($expand=Category)
// OData v2/v3: $expand=Products/Category

// Multiple expands
.Expand(s => s.Products)
    .ThenExpand(products => products.Select(p => p.Category))
.Expand(s => s.Address)
.Expand(s => s.Orders)
// OData v4: $expand=Products($expand=Category),Address,Orders
// OData v2/v3: $expand=Products/Category,Address,Orders
```

### Ordering

Order results with multi-level sorting:

```csharp
// Simple ascending order
.Order(p => p.Name)
// Generates: $orderby=Name

// Descending order
.OrderDescending(p => p.Price)
// Generates: $orderby=Price desc

// Multiple order levels
.Order(p => p.Country)
    .ThenByDescending(p => p.Rating)
    .ThenBy(p => p.Name)
// Generates: $orderby=Country,Rating desc,Name

// Nested property ordering
.Order(p => p.Category.Name)
    .ThenBy(p => p.Price)
// Generates: $orderby=Category/Name,Price
```

### Pagination

Control result sets with Top and Skip:

```csharp
// First 20 results
.Top(20)
// Generates: $top=20

// Skip first 10, take next 20
.Skip(10)
.Top(20)
// Generates: $skip=10&$top=20

// Page 3 of results (20 per page)
.Skip(40)
.Top(20)
```

### Counting

Get total count of results:

```csharp
// Include count in response
.Count(true)
// Generates: $count=true

// Query with count
var result = await client
    .Query<Product>()
    .Filter(p => p.InStock)
    .Count(true)
    .ExecuteAsync();

// Access count from response
Console.WriteLine($"Total products: {result.Count}");
```

### Selection (Projection)

Select specific properties:

```csharp
// Select specific properties
.Select()
// Note: Advanced select projection is part of the library
```

## 🔨 CRUD Operations

### Creating Entities

```csharp
// Define input class derived from ODataInputBase
public class ProductInput : ODataInputBase
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public CategoryInput? Category { get; set; }
}

// Create entity
var input = new ProductInput 
{ 
    Name = "New Product", 
    Price = 99.99m 
};

var created = await client.CreateEntityAsync<Product>("Products", input);
```

### Updating Entities

```csharp
var input = new ProductInput 
{ 
    Price = 89.99m 
};

bool updated = await client.UpdateEntityAsync("Products", "ID=123", input);
```

### Deleting Entities

```csharp
bool deleted = await client.DeleteEntityAsync("Products", "ID=123");
```

### Querying Single Entity

```csharp
var product = await client.GetEntityAsync<Product>("Products", "ID=123", 
    select: "ID,Name,Price", 
    expand: "Category");
```

## 🔧 Client Generation

Generate type-safe client code from OData metadata:

```csharp
using Linq2OData.Generator;
using Linq2OData.Generator.Models;

// Load metadata XML
var metadataXml = await httpClient.GetStringAsync("$metadata");

// Configure generation
var request = new ClientRequest
{
    Name = "MyODataClient",
    Namespace = "MyApp.OData"
};
request.AddMetadata(metadataXml);

// Generate client files
var generator = new ClientGenerator(request);
var files = generator.GenerateClient();

// Or write to disk
generator.GenerateClient(outputFolder: "./Generated");
```

This generates:
- `MyODataClient.cs` - The main client with typed query builders
- Entity type classes in `Types/` folder
- Input classes for create/update operations
- Enum types

## 📚 Advanced Features

### Working with Different Date Formats

Linq2OData handles OData v2/v3 Microsoft date formats (`/Date(...)/) automatically:

```csharp
// OData v2/v3 automatically handles: "/Date(1234567890000)/"
// OData v4 uses standard ISO 8601 dates
var recentProducts = await client
    .Query<Product>()
    .Filter(p => p.CreatedDate > DateTime.Now.AddMonths(-3))
    .ExecuteAsync();
```

### Navigation Property Serialization

OData v2/v3 requires collections to be wrapped in `"results"`:

```csharp
// For OData v2/v3, this is automatically serialized as:
// { "Products": { "results": [...] } }

// For OData v4:
// { "Products": [...] }
```

### Custom JSON Options

```csharp
var client = new ODataClient(httpClient, ODataVersion.V4);

// Access and customize JSON serialization
client.JsonOptions.PropertyNameCaseInsensitive = true;
client.JsonOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
```

### Error Handling

```csharp
try
{
    var products = await client
        .Query<Product>()
        .Filter(p => p.ID == 123)
        .ExecuteAsync();
}
catch (ODataRequestException ex)
{
    Console.WriteLine($"OData Error: {ex.Message}");
    Console.WriteLine($"Status: {ex.StatusCode}");
    Console.WriteLine($"Details: {ex.ResponseContent}");
}
```

## 🎨 API Naming Convention

Linq2OData uses an **OData-centric** naming convention for primary operations:

| OData Parameter | API Method | Example |
|----------------|------------|---------|
| `$filter` | `.Filter()` | `.Filter(p => p.Price > 10)` |
| `$expand` | `.Expand()` | `.Expand(s => s.Products)` |
| `$orderby` | `.Order()` / `.OrderDescending()` | `.Order(p => p.Name)` |
| `$top` | `.Top()` | `.Top(20)` |
| `$skip` | `.Skip()` | `.Skip(10)` |
| `$count` | `.Count()` | `.Count(true)` |

**Secondary operations** use familiar LINQ patterns:
- `.ThenBy()` / `.ThenByDescending()` - for additional ordering
- `.ThenExpand()` - for nested expansions

## 🧪 Testing

The library includes comprehensive tests:

```bash
# Run all tests
dotnet test

# Run specific test category
dotnet test --filter "FullyQualifiedName~FilterExpressionTests"
```

## 🌐 Real-World Examples

### E-commerce Product Catalog

```csharp
var topSellingProducts = await client
    .Query<Product>()
    .Expand(p => p.Category)
    .Expand(p => p.Manufacturer)
    .Expand(p => p.Reviews)
        .ThenExpand(reviews => reviews.Select(r => r.Customer))
    .Filter(p => p.InStock && p.Rating >= 4 && p.Price < 500)
    .Order(p => p.SalesCount)
        .ThenByDescending(p => p.Rating)
        .ThenBy(p => p.Price)
    .Top(50)
    .ExecuteAsync();
```

### Customer Orders Report

```csharp
var recentOrders = await client
    .Query<Order>()
    .Expand(o => o.Customer)
        .ThenExpand(c => c.Address)
    .Expand(o => o.OrderItems)
        .ThenExpand(items => items.Select(i => i.Product))
    .Filter(o => o.OrderDate > DateTime.Now.AddMonths(-1) && 
                 o.Status != "Cancelled" &&
                 o.TotalAmount > 100)
    .OrderDescending(o => o.OrderDate)
        .ThenBy(o => o.Customer.Name)
    .Skip(0)
    .Top(100)
    .Count(true)
    .ExecuteAsync();

Console.WriteLine($"Found {recentOrders.Count} orders");
```

### Supplier Inventory Management

```csharp
var usSuppliers = await client
    .Query<Supplier>()
    .Expand(s => s.Products)
        .ThenExpand(products => products.Select(p => p.Category))
        .ThenExpand(categories => categories.Select(c => c.Department))
    .Expand(s => s.Locations)
        .ThenExpand(locations => locations.Select(l => l.Address))
    .Filter(s => s.Country == "USA" && 
                 s.IsActive && 
                 s.Rating >= 4.0)
    .Order(s => s.State)
        .ThenByDescending(s => s.Rating)
        .ThenBy(s => s.Name)
    .Top(100)
    .ExecuteAsync();
```

## 📝 Documentation

Additional documentation is available in the `/docs` folder:

- [OData Version Support](docs/ODataVersionSupport.md) - Details on version-specific features
- [Navigation Property Serialization](docs/NavigationPropertySerialization.md) - How collections are serialized
- [Naming Convention](docs/NamingConvention.md) - API design philosophy
- [Order By Support](docs/OrderBySupport.md) - Advanced ordering scenarios
- [Expand Examples](docs/ExpandExamples.cs) - Comprehensive expand examples
- [Order By Examples](docs/OrderByExamples.cs) - Comprehensive ordering examples

Visit the [project website](https://joadan.github.io/Linq2OData/) for interactive examples and more documentation.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

### Development Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/joadan/Linq2OData.git
   ```

2. Build the solution:
   ```bash
   dotnet build
   ```

3. Run tests:
   ```bash
   dotnet test
   ```

## 📄 License

Copyright 2026 (c) Joakim Dangården. All rights reserved.

## 🙏 Acknowledgments

- Built for the .NET community
- Inspired by the OData specification and LINQ patterns
- Thanks to all contributors

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/joadan/Linq2OData/issues)
- **Documentation**: [Project Website](https://joadan.github.io/Linq2OData/)
- **NuGet**: [Linq2OData.Core](https://www.nuget.org/packages/Linq2OData.Core/)

---

Made with ❤️ by Joakim Dangården