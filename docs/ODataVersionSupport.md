# OData Version Support for Expand

The expand functionality automatically adapts to the OData version configured in your `ODataClient`.

## Syntax Differences

### Simple Expand (Same in all versions)
```csharp
.Expand(e => e.Orders)
```
- **OData v2/v3**: `$expand=Orders`
- **OData v4**: `$expand=Orders`

### Nested Expand (Different syntax)
```csharp
.Expand(e => e.Orders)
    .ThenExpand(orders => orders.Select(o => o.OrderDetails))
    .ThenExpand(details => details.Select(d => d.Product))
```
- **OData v2/v3**: `$expand=Orders/OrderDetails/Product` (forward slashes)
- **OData v4**: `$expand=Orders($expand=OrderDetails($expand=Product))` (nested parentheses)

### Multiple Expands (Same in all versions)
```csharp
.Expand(e => e.Orders)
.Expand(e => e.Customer)
.Expand(e => e.Address)
```
- **OData v2/v3**: `$expand=Orders,Customer,Address`
- **OData v4**: `$expand=Orders,Customer,Address`

### Complex Example
```csharp
await client
    .Query<Supplier>()
    .Expand(s => s.Products)
        .ThenExpand(products => products.Select(p => p.Category))
    .Expand(s => s.Address)
        .ThenExpand(a => a.Country)
    .Filter(s => s.IsActive)
    .ExecuteAsync();
```

**OData v2/v3 Output:**
```
/Suppliers?$expand=Products/Category,Address/Country&$filter=(IsActive eq true)
```

**OData v4 Output:**
```
/Suppliers?$expand=Products($expand=Category),Address($expand=Country)&$filter=(IsActive eq true)
```

## How It Works

The expand builder automatically detects the OData version from the `ODataClient` instance and generates the appropriate syntax:

1. **OData v2/v3**: Uses forward slashes (`/`) to separate nested navigation properties
2. **OData v4**: Uses the `$expand=` parameter within parentheses for nested expands

You don't need to change your code when switching between OData versions - the library handles it automatically!

## Configuration

```csharp
// OData v2
var clientV2 = new ODataClient(httpClient, ODataVersion.V2);

// OData v3
var clientV3 = new ODataClient(httpClient, ODataVersion.V3);

// OData v4
var clientV4 = new ODataClient(httpClient, ODataVersion.V4);
```

All expand expressions will automatically use the correct syntax for the configured version.
