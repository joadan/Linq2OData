# Order and ThenBy Expression Support

## Overview

Added LINQ expression-based support for OData `$orderby` queries, similar to the Expand functionality. This allows type-safe, IntelliSense-friendly ordering of query results.

## Implementation

### Components Created

1. **ODataOrderByVisitor** (`src\Linq2OData.Core\Expressions\OrderByExpressionVisitor.cs`)
   - Converts LINQ expressions to OData orderby property names
   - Handles nested properties (e.g., `s => s.Address.City` → `Address/City`)
   - Supports appending multiple orderby clauses

2. **OrderByBuilder** (`src\Linq2OData.Core\Builders\OrderByBuilder.cs`)
   - Fluent API for chaining orderby operations
   - `ThenBy()` - adds ascending orderby
   - `ThenByDescending()` - adds descending orderby
   - All QueryBuilder methods for continued chaining

3. **QueryBuilder Updates**
   - Added `orderby` field
   - Added `Order<TProperty>(Expression)` - returns OrderByBuilder
   - Added `OrderDescending<TProperty>(Expression)` - returns OrderByBuilder
   - Added `Order(string)` - string-based overload
   - Added `AppendOrderBy()` internal method

4. **ODataClient Updates**
   - Added `orderby` parameter to `QueryEntitySetAsync`
   - Added `orderby` parameter to `GenerateUrl`
   - Generates `$orderby=` query parameter

## Usage Examples

### Simple Order
```csharp
var result = await client
    .Query<Supplier>()
    .Order(s => s.Name)
    .ExecuteAsync();
// Generates: $orderby=Name
```

### OrderDescending
```csharp
var result = await client
    .Query<Supplier>()
    .OrderDescending(s => s.Rating)
    .ExecuteAsync();
// Generates: $orderby=Rating desc
```

### Multiple Order with ThenBy
```csharp
var result = await client
    .Query<Supplier>()
    .Order(s => s.Country)
        .ThenBy(s => s.Rating)
        .ThenByDescending(s => s.JoinDate)
    .ExecuteAsync();
// Generates: $orderby=Country,Rating,JoinDate desc
```

### Nested Properties
```csharp
var result = await client
    .Query<Supplier>()
    .Order(s => s.Address.City)
        .ThenBy(s => s.Name)
    .ExecuteAsync();
// Generates: $orderby=Address/City,Name
```

### Combined with Other Operations
```csharp
var result = await client
    .Query<Supplier>()
    .Filter(s => s.IsActive)
    .OrderDescending(s => s.Rating)
        .ThenBy(s => s.Name)
    .Expand(s => s.Products)
    .Top(20)
    .Skip(10)
    .ExecuteAsync();
// Generates: $filter=(IsActive eq true)&$orderby=Rating desc,Name&$expand=Products&$top=20&$skip=10
```

## OData Syntax Generated

| Usage | OData Query |
|-------|-------------|
| `.Order(s => s.Name)` | `$orderby=Name` |
| `.OrderDescending(s => s.Price)` | `$orderby=Price desc` |
| `.Order(s => s.Country).ThenBy(s => s.Name)` | `$orderby=Country,Name` |
| `.Order(s => s.Rating).ThenByDescending(s => s.Date)` | `$orderby=Rating,Date desc` |
| `.Order(s => s.Address.City)` | `$orderby=Address/City` |

## Tests

Created comprehensive test suite with **21 passing tests** covering:

### ODataOrderByVisitor Tests (5 tests)
- ✅ Simple property conversion
- ✅ Nested property paths
- ✅ AppendOrderBy ascending/descending
- ✅ Multiple properties with comma separation

### QueryBuilder Order Tests (3 tests)
- ✅ Order sets orderby string
- ✅ OrderDescending adds " desc"
- ✅ String overload support

### OrderByBuilder ThenBy Tests (4 tests)
- ✅ ThenBy appends properties
- ✅ ThenByDescending adds descending
- ✅ Multiple ThenBy chains
- ✅ OrderDescending with ThenBy

### Method Chaining Tests (5 tests)
- ✅ Filter returns QueryBuilder
- ✅ Top returns QueryBuilder
- ✅ Skip returns QueryBuilder
- ✅ Count returns QueryBuilder
- ✅ Implicit conversion works

### Nested Property Tests (2 tests)
- ✅ Single nested property
- ✅ Multiple nested properties with mixed ordering

### Integration Tests (2 tests)
- ✅ Order with Filter and Top
- ✅ Complex query with all operations

## Key Features

- ✅ **Type-safe**: Compile-time validation of property names
- ✅ **IntelliSense support**: Auto-completion for properties
- ✅ **Fluent API**: Natural chaining with OData-centric naming
- ✅ **Nested properties**: Support for path-based ordering
- ✅ **Ascending/Descending**: Both order directions supported
- ✅ **Multiple orderings**: Chain unlimited ThenBy operations
- ✅ **Backward compatible**: String-based Order still available
- ✅ **Integrates seamlessly**: Works with Filter, Expand, Top, Skip, etc.
- ✅ **OData-centric naming**: Consistent with Filter, Expand, Top, Skip, Count

## Naming Convention

Following the established OData-centric API pattern:
- `Order()` instead of `OrderBy()` - consistent with `Filter`, `Expand`, `Top`
- `OrderDescending()` instead of `OrderByDescending()`
- `ThenBy()` and `ThenByDescending()` - kept for LINQ familiarity

This maintains consistency with OData terminology while leveraging familiar LINQ chaining patterns.

## Comparison with LINQ

The API mirrors LINQ's OrderBy/ThenBy pattern but with OData-centric naming:

```csharp
// LINQ
var result = items
    .OrderBy(i => i.Category)
    .ThenByDescending(i => i.Price)
    .ThenBy(i => i.Name)
    .ToList();

// OData Query Builder (similar pattern, OData naming!)
var result = await client
    .Query<Item>()
    .Order(i => i.Category)
    .ThenByDescending(i => i.Price)
    .ThenBy(i => i.Name)
    .ExecuteAsync();
```

## Performance

- No reflection at runtime
- Expression trees compiled once and reused
- Minimal overhead compared to string-based queries
- Efficient string building with StringBuilder

## Future Enhancements

Potential additions:
- Support for collection ordering (if OData spec allows)
- Custom ordering comparers
- Case-insensitive ordering

## Related Functionality

This complements the existing expression-based features:
- **Expand/ThenExpand**: For navigation property expansion
- **Filter**: For WHERE clauses
- **Select**: For projection (coming soon?)
