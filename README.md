# Linq2OData

[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/)
[![NuGet](https://img.shields.io/badge/NuGet-Linq2OData-orange.svg)](https://www.nuget.org/packages/Linq2OData.Core/)

A modern, type-safe .NET library for building OData queries using LINQ expressions. Supports OData v2, v3, and v4 with automatic syntax adaptation.

## ✨ Features

- **Type-Safe LINQ Queries** - Write strongly-typed C# expressions, not error-prone strings
- **Auto Client Generation** - Generate typed client from OData `$metadata` 
- **Multi-Version Support** - Works with OData v2, v3, and v4 seamlessly
- **Full OData Support** - `$filter`, `$expand`, `$orderby`, `$top`, `$skip`, `$count`, `$select`

## 📦 Installation

```bash
dotnet add package Linq2OData.Core
dotnet add package Linq2OData.Generator
```

## 🚀 Quick Start

### 1. Generate Client from Metadata

The typical workflow is to generate a type-safe client from your OData service's `$metadata`:

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

This generates:
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
        .ThenExpand(products => products.Select(p => p.Category))
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
// Simple expand
.Expand(s => s.Orders)

// Nested expands (automatically handles v2/v3 vs v4 syntax)
.Expand(o => o.Customer)
    .ThenExpand(c => c.Address)

// Collection expands
.Expand(s => s.Products)
    .ThenExpand(products => products.Select(p => p.Category))

// Multiple expands
.Expand(s => s.Products)
.Expand(s => s.Address)
```

**OData v4**: `$expand=Products($expand=Category)`  
**OData v2/v3**: `$expand=Products/Category`  
*The library handles this automatically based on your service version*

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