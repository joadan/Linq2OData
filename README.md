> [!WARNING]
> Under development!
> API is frequently updated, documentation may not be correct.

# Linq2OData

[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/)
[![NuGet](https://img.shields.io/badge/NuGet-Linq2OData-orange.svg)](https://www.nuget.org/packages/Linq2OData.Core/)

A modern, type-safe .NET library for building OData queries using LINQ expressions. Supports OData v2, v3, and v4 with automatic syntax adaptation.

## ✨ Features

- **Type-Safe LINQ Queries** - Write strongly-typed C# expressions, not error-prone strings
- **Auto Client Generation** - Generate typed client from OData `$metadata` 
- **Smart Projections** - Use `.Select()` to retrieve only the fields you need with deep nesting support
- **Multi-Version Support** - Works with OData v2, v3, and v4 seamlessly
- **Full OData Support** - `$filter`, `$expand`, `$orderby`, `$top`, `$skip`, `$count`, `$select`

## 📦 Installation

```bash
dotnet add package Linq2OData.Core
dotnet add package Linq2OData.Generator
```

## 🚀 Quick Start

### 1. Generate Client from Metadata

The typical workflow is to generate a type-safe client from your OData service's `$metadata`.

**Option A: Use the Web Generator**

Visit **[https://joadan.github.io/Linq2OData/generate-client](https://joadan.github.io/Linq2OData/generate-client)** to:
- Upload your `$metadata` XML file(s)
- Configure client name and namespace
- Download generated client as a ZIP file

**Option B: Programmatic Generation**

```csharp
using Linq2OData.Generator;
using Linq2OData.Generator.Models;

// Fetch metadata from your OData service
var httpClient = new HttpClient();
var metadataXml = await httpClient.GetStringAsync("https://your-api.com/odata/$metadata");

// Configure and generate client
var request = new ClientRequest
{
    Name = "MyODataClient",
    Namespace = "MyApp.OData"
};
request.AddMetadata(metadataXml);

var generator = new ClientGenerator(request);
generator.GenerateClient(outputFolder: "./Generated");
```

**What Gets Generated:**
- **Client class** with typed query builders (`MyODataClient.cs`)
- **Entity types** in `Types/` folder - all your data models
- **Input types** in `Inputs/` folder - for create/update operations  
- **Enums** - any enumeration types from metadata

### 2. Use the Generated Client

```csharp
// Initialize with HttpClient
var httpClient = new HttpClient 
{ 
    BaseAddress = new Uri("https://your-api.com/odata/")
};
var client = new MyODataClient(httpClient);

// Query with type-safe LINQ expressions
var products = await client
    .Query<Product>()
    .Filter(p => p.Price > 100 && p.InStock)
    .Order(p => p.Name)
    .Top(20)
    .ExecuteAsync();

// Complex queries with expands
var suppliers = await client
    .Query<Supplier>()
    .Expand(s => s.Products)
    .Filter(s => s.Country == "USA")
    .ExecuteAsync();
```

## 🎯 Common Operations

### Filtering

```csharp
// Comparisons and logical operators
.Filter(p => p.Price > 10 && p.Price < 100)

// String operations  
.Filter(p => p.Name.Contains("Phone") || p.Name.StartsWith("Apple"))

// Navigation properties
.Filter(p => p.Category.Name == "Electronics")

// Date comparisons
.Filter(p => p.CreatedDate > DateTime.Now.AddMonths(-6))
```

### Expanding Related Data

```csharp
// Expand single navigation property
.Expand(s => s.Address)

// Expand collection navigation property
.Expand(s => s.Orders)

// Nested expand - chain properties directly
.Expand(s => s.Customer.Address)
.Expand(s => s.Customer.Address.Country)

// Multiple root-level expands
.Expand(s => s.Products)
.Expand(s => s.Address)
.Expand(s => s.Orders)
```

**Note:** Each `.Expand()` call adds another navigation property to expand. For nested navigation, chain properties using dot notation (e.g., `s.Customer.Address.Country`).

**OData Version Differences (handled automatically):**  
- **v4**: `$expand=Customer($expand=Address($expand=Country))`  
- **v2/v3**: `$expand=Customer/Address/Country`

### Projections (Select Specific Fields)

Use `.Select()` to retrieve only the fields you need, reducing payload size and improving performance. This generates the OData `$select` and `$expand` query parameters.

> [!IMPORTANT]
> **Server vs Client Evaluation:** Only `$select` and `$expand` are sent to the OData server. Any filtering, ordering, or computed properties within the `.Select()` expression are evaluated **client-side** after the data is retrieved.

#### Basic Property Selection

```csharp
// Select specific properties
var products = await client
    .Query<Product>()
    .Select(p => new { p.Name, p.Price })
    .ExecuteAsync();
// Generates: $select=Name,Price
```

#### Selecting with Navigation Properties

```csharp
// Include a navigation property alongside simple properties
var products = await client
    .Query<Product>()
    .Select(p => new 
    { 
        p.Name, 
        p.Price, 
        p.Category  // Full Category object included
    })
    .ExecuteAsync();
// Generates: $select=Name,Price&$expand=Category
```

#### Nested Property Access

```csharp
// Access properties from nested navigation objects
var products = await client
    .Query<Product>()
    .Select(p => new 
    { 
        p.Name, 
        p.Price,
        CategoryName = p.Category.Name  // Only Category.Name
    })
    .ExecuteAsync();
// Generates: $select=Name,Price&$expand=Category($select=Name)
```

#### Deep Nesting

```csharp
// Access deeply nested properties
var orders = await client
    .Query<Order>()
    .Select(o => new 
    { 
        o.OrderNumber,
        CustomerCountry = o.Customer.Address.Country.Name
    })
    .ExecuteAsync();
// OData v4: $select=OrderNumber&$expand=Customer($expand=Address($expand=Country($select=Name)))
// OData v2/v3: $select=OrderNumber,Customer&$expand=Customer/Address/Country
```

#### Complex Projections

```csharp
// Mix simple properties, nested properties, and full objects
var suppliers = await client
    .Query<Supplier>()
    .Select(s => new 
    { 
        s.Name,
        s.Email,
        PrimaryContactName = s.PrimaryContact.FullName,
        PrimaryContactPhone = s.PrimaryContact.Phone,
        AllProducts = s.Products  // Full collection
    })
    .ExecuteAsync();
// Generates: $select=Name,Email
//           &$expand=PrimaryContact($select=FullName,Phone);Products
```

#### Use with Single Entity Retrieval

```csharp
// Project when getting a single entity
var person = await client
    .Get<Person>(p => p.ID = 5)
    .Select(p => new 
    { 
        p.Name, 
        p.Email,
        DetailPerson = p.PersonDetail.Person  // Nested navigation
    })
    .ExecuteAsync();
// Generates: Persons(ID=5)?$select=Name,Email&$expand=PersonDetail($expand=Person)
```

**Benefits of Projections:**
- 🚀 **Performance** - Retrieve only needed data, reducing bandwidth
- 📦 **Smaller Payloads** - Less data transferred over the network
- 🎯 **Type-Safe** - Compile-time checking of property names
- 🔄 **Version Agnostic** - Library handles OData v2/v3/v4 differences automatically

**Note:** The null-forgiving operator (`!`) is supported for nullable navigation properties:
```csharp
.Select(p => new { Name = p.Category!.Name })  // Works correctly
```

#### Projecting to Custom Types (DTOs)

You can project to your own classes instead of anonymous types:

```csharp
// Define a DTO/view model
public class ProductSummaryDto
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string CategoryName { get; set; }
    public decimal DiscountPrice { get; set; }
}

// Project to the custom type
var products = await client
    .Query<Product>()
    .Filter(p => p.Price > 100)
    .Select(p => new ProductSummaryDto
    {
        Name = p.Name,                           // ✅ Server sends
        Price = p.Price,                         // ✅ Server sends
        CategoryName = p.Category.Name,          // ✅ Server sends via $expand
        DiscountPrice = p.Price * 0.9m           // ⚠️ Computed client-side
    })
    .ExecuteAsync();
// Returns List<ProductSummaryDto> - perfect for APIs, serialization, etc.
```

#### Server-Side vs Client-Side Operations

```csharp
// ✅ OPTIMAL - Filter, order, paginate, then project
var products = await client
    .Query<Product>()
    .Filter(p => p.Price > 100)           // ✅ Server-side filter
    .Order(p => p.Name)                   // ✅ Server-side ordering
    .Top(50)                              // ✅ Server-side limit
    .Skip(10)                             // ✅ Server-side pagination
    .Select(p => new { p.Name, p.Price }) // ✅ Server-side projection
    .ExecuteAsync();
// URL: Products?$filter=Price gt 100&$orderby=Name&$top=50&$skip=10&$select=Name,Price

// ⚠️ NOTE - Computed properties are evaluated client-side
var products = await client
    .Query<Product>()
    .Select(p => new 
    { 
        FullName = p.FirstName + " " + p.LastName,  // ⚠️ Client-side computation
        DiscountPrice = p.Price * 0.9m              // ⚠️ Client-side computation
    })
    .ExecuteAsync();
// URL: Products (fetches ALL products, ALL fields, then computes client-side)
// Use case: When you need computations that OData doesn't support server-side

// ✅ BEST OF BOTH WORLDS - Server projects minimal fields, computations done client-side
var result = await client
    .Query<Product>()
    .Filter(p => p.Price > 100)                  // ✅ Server filters
    .Select(p => new 
    { 
        p.FirstName,                             // ✅ Server sends this field
        p.LastName,                              // ✅ Server sends this field
        p.Price,                                 // ✅ Server sends this field
        FullName = p.FirstName + " " + p.LastName,  // ⚠️ Computed client-side
        DiscountPrice = p.Price * 0.9m           // ⚠️ Computed client-side
    })
    .ExecuteAsync();
// Server sends only: FirstName, LastName, Price
// Client computes: FullName, DiscountPrice
// You get everything in one result!
```

### Ordering & Pagination

```csharp
// Order by multiple fields
.Order(p => p.Category.Name)
    .ThenByDescending(p => p.Rating)
    .ThenBy(p => p.Price)

// Pagination
.Skip(20)
.Top(10)

// Get total count
.Count(true)
```

### CRUD Operations

All types and inputs are generated automatically:

```csharp
// Create (using generated input type)
var input = new ProductInput 
{ 
    Name = "New Product", 
    Price = 99.99m 
};
var created = await client
    .Create<Product>()
    .WithInput(input)
    .ExecuteAsync();

// Update
await client
    .Update<Product>(p => p.ID = 123)
    .WithInput(input)
    .ExecuteAsync();

// Delete
await client
    .Delete<Product>(p => p.ID = 123)
    .ExecuteAsync();

// Get single entity
var product = await client
    .Get<Product>(p => p.ID = 123)
    .Expand(p => p.Category)
    .ExecuteAsync();
```

## 🔄 OData Version Support

The library automatically adapts to your OData version. Key differences handled:

| Feature | OData v2/v3 | OData v4 |
|---------|-------------|----------|
| Nested expand | `Products/Category` | `Products($expand=Category)` |
| Date format | `/Date(1234567890000)/` | ISO 8601 |
| Collections | Wrapped in `"results"` | Direct arrays |

All handled transparently by the library.

## 📖 Documentation

For detailed documentation and examples:

- **[Project Website](https://joadan.github.io/Linq2OData/)** - Interactive examples
- **[Projection Support](docs/ProjectionSupport.md)** - Complete guide to using Select projections
- **[OData Version Support](docs/ODataVersionSupport.md)** - Version-specific features
- **[API Naming Convention](docs/NamingConvention.md)** - Design philosophy
- **[More Examples](docs/)** - Expand, OrderBy, and advanced scenarios

## 🛠️ Advanced Usage

### Manual Client Setup (without generator)

If you need to use the library without code generation:

```csharp
// Define entity manually
[ODataEntitySet("Products")]
public class Product : IODataEntitySet
{
    public int ID { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string __Key => $"ID={ID}";
}

// Use ODataClient directly
var client = new ODataClient(httpClient, ODataVersion.V4);
var result = await client.QueryEntitySetAsync<Product>(...);
```

### Custom JSON Serialization

```csharp
var client = new MyODataClient(httpClient);
client.ODataClient.JsonOptions.PropertyNameCaseInsensitive = true;
```

## 🤝 Contributing

Contributions welcome! See [GitHub Issues](https://github.com/joadan/Linq2OData/issues) for open items.

```bash
git clone https://github.com/joadan/Linq2OData.git
dotnet build
dotnet test
```

## 📄 License

Copyright 2026 (c) Joakim Dangården. All rights reserved.

---

**[Documentation](https://joadan.github.io/Linq2OData/)** • **[NuGet](https://www.nuget.org/packages/Linq2OData.Core/)** • **[Issues](https://github.com/joadan/Linq2OData/issues)**
