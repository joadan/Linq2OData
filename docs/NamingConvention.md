# API Naming Convention

## OData-Centric Approach

Linq2OData follows an **OData-centric naming convention** for its fluent API methods to maintain consistency with OData terminology.

## Naming Pattern

| OData Query Parameter | API Method | Rationale |
|----------------------|------------|-----------|
| `$filter` | `.Filter()` | Matches OData terminology |
| `$expand` | `.Expand()` | Matches OData terminology |
| `$top` | `.Top()` | Matches OData terminology |
| `$skip` | `.Skip()` | Matches OData terminology |
| `$count` | `.Count()` | Matches OData terminology |
| `$orderby` | `.Order()` | Shortened form of OData `$orderby` |
| `$orderby=x desc` | `.OrderDescending()` | Shortened form with direction |

## Rationale

### Why Not LINQ Names?

While LINQ uses `Where`, `OrderBy`, `Take`, etc., this library uses OData-centric names:

```csharp
// ❌ NOT using LINQ naming
.Where(s => s.IsActive)    // LINQ
.OrderBy(s => s.Name)      // LINQ
.Take(10)                  // LINQ

// ✅ Using OData-centric naming
.Filter(s => s.IsActive)   // OData
.Order(s => s.Name)        // OData
.Top(10)                   // OData
```

**Benefits:**
1. **Consistency**: All methods match OData terminology
2. **Clarity**: Developers working with OData understand the direct mapping
3. **Predictability**: Method names directly correspond to query parameters
4. **Documentation**: Easier to map to OData specification

### Hybrid Approach for Chaining

For secondary operations, we keep familiar LINQ patterns:

```csharp
.Order(s => s.Country)
    .ThenBy(s => s.Name)           // Kept ThenBy (LINQ familiar)
    .ThenByDescending(s => s.Date) // Kept ThenByDescending

.Expand(s => s.Products)
    .ThenExpand(p => p.Select(...)) // Kept ThenExpand pattern
```

**Why keep "Then" methods?**
- Widely recognized LINQ pattern
- Clear semantic meaning (secondary/subsequent operation)
- Good IntelliSense discoverability

## Complete API Overview

### Primary Operations (OData-Centric)

```csharp
client.Query<Entity>()
    .Filter(e => e.IsActive)           // OData: $filter
    .Expand(e => e.NavProperty)        // OData: $expand
    .Order(e => e.PropertyName)        // OData: $orderby
    .OrderDescending(e => e.Price)     // OData: $orderby=Price desc
    .Top(20)                           // OData: $top
    .Skip(10)                          // OData: $skip
    .Count(true)                       // OData: $count
    .ExecuteAsync();
```

### Secondary Operations (LINQ-Familiar)

```csharp
// After Expand
.Expand(e => e.Products)
    .ThenExpand(p => p.Select(x => x.Category))  // Nested expand

// After Order
.Order(e => e.Country)
    .ThenBy(e => e.Name)                         // Additional ordering
    .ThenByDescending(e => e.Date)               // Descending ordering
```

## Design Philosophy

1. **Primary methods** = OData terminology
   - Entry points for operations
   - Map directly to OData query parameters
   
2. **Chaining methods** = LINQ patterns
   - Secondary operations
   - Familiar to .NET developers
   - Clear semantic meaning

## Examples

### Full Query with Both Conventions

```csharp
var results = await client
    .Query<Supplier>()
    .Filter(s => s.Country == "USA")           // OData-centric
    .Expand(s => s.Products)                   // OData-centric
        .ThenExpand(p => p.Select(x => x.Category))  // LINQ-familiar
    .Order(s => s.Rating)                      // OData-centric
        .ThenByDescending(s => s.JoinDate)     // LINQ-familiar
        .ThenBy(s => s.Name)                   // LINQ-familiar
    .Top(20)                                   // OData-centric
    .Skip(10)                                  // OData-centric
    .Count(true)                               // OData-centric
    .ExecuteAsync();
```

Generates:
```
$filter=(Country eq 'USA')
&$expand=Products($expand=Category)
&$orderby=Rating,JoinDate desc,Name
&$top=20
&$skip=10
&$count=true
```

## Migration Notes

If you were using the LINQ-style naming (`OrderBy`/`OrderByDescending`), update to:

```csharp
// Before
.OrderBy(s => s.Name)
.OrderByDescending(s => s.Price)

// After
.Order(s => s.Name)
.OrderDescending(s => s.Price)

// ThenBy/ThenByDescending remain unchanged
.ThenBy(s => s.Name)
.ThenByDescending(s => s.Date)
```

## Summary

✅ **Primary operations**: OData-centric (`Filter`, `Expand`, `Order`, `Top`, `Skip`, `Count`)  
✅ **Chaining operations**: LINQ-familiar (`ThenBy`, `ThenExpand`)  
✅ **Result**: Best of both worlds - predictable OData mapping with familiar .NET patterns
