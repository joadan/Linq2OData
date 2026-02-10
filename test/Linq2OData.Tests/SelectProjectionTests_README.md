# SelectProjectionTests

This test suite validates the LINQ Select projection functionality and OData `$select`/`$expand` query generation in the Linq2OData library.

## Purpose

These tests ensure that when you write LINQ expressions like:

```csharp
.Select(e => new { e.Name, e.ID, PersonDetail = e.PersonDetail!.Person })
```

The library correctly generates OData query strings such as:
- `$select=Name,ID`
- `$expand=PersonDetail($expand=Person)`

## Test Categories

### 1. Simple Property Selection Tests
Tests basic property selection without navigation properties.

**Example:**
```csharp
Expression: p => new { p.Name, p.ID }
Expected (V4): $select=Name,ID
```

### 2. Simple Expand Tests
Tests selection with single-level navigation properties.

**Example:**
```csharp
Expression: p => new { p.Name, p.PersonDetail }
Expected (V4): $select=Name&$expand=PersonDetail
```

### 3. Nested Expand Tests (Critical Bug Fix Scenario)
Tests the most important scenario - nested navigation property access.

**The Bug That Was Fixed:**
Before the fix, expressions like `p.PersonDetail!.Person` would generate invalid OData:
```
❌ BAD: $expand=PersonDetail(Person)  // Invalid OData syntax
```

After the fix, it correctly generates:
```
✅ GOOD: $expand=PersonDetail($expand=Person)  // Valid OData v4 syntax
```

**Key Tests:**
- `SelectProjection_NestedExpand_V4_GeneratesCorrectExpandWithPrefix` - The primary bug fix test
- `SelectProjection_NestedExpandWithProperties_V4_GeneratesCorrectSelectAndExpand` - Nested with property selection
- `SelectProjection_NestedExpandMultipleLevels_V4_GeneratesCorrectExpand` - Multiple nesting levels
- `SelectProjection_MultipleNestedExpands_V4_GeneratesCorrectExpand` - Multiple nested paths

### 4. Complex Scenarios
Tests real-world combinations of properties and navigation properties.

**Example:**
```csharp
Expression: p => new 
{ 
    p.Name, 
    p.Price, 
    CategoryName = p.Category!.Name,
    CategoryProducts = p.Category!.Products 
}
Expected (V4): 
  $select=Name,Price
  $expand=Category($select=Name;$expand=Products)
```

### 5. Edge Cases
Tests boundary conditions and special cases:
- Empty projections
- Only complex properties
- Null-forgiving operator (`!`) handling

## OData Version Differences

The tests cover both OData V2/V3 and V4, which have different syntax requirements:

### OData V4
- Uses nested parentheses with `$expand=` prefix: `PersonDetail($expand=Person)`
- Properties use `$select=` prefix: `PersonDetail($select=Name)`
- Separates select and expand with semicolon: `($select=Name;$expand=Person)`

### OData V2/V3
- Uses slash notation for nesting: `PersonDetail/Person`
- No `$expand=` prefix in nested paths
- Complex properties must appear in both `$select` and `$expand`

## Running the Tests

Run all projection tests:
```bash
dotnet test --filter "FullyQualifiedName~SelectProjectionTests"
```

Run a specific test:
```bash
dotnet test --filter "FullyQualifiedName~SelectProjection_NestedExpand_V4_GeneratesCorrectExpandWithPrefix"
```

## Test Structure

Each test follows this pattern:

```csharp
[Fact]
public void TestName_Scenario_Expected()
{
    // Arrange - Create the expression visitor and LINQ expression
    var visitor = new SelectExpressionVisitor();
    Expression<Func<TEntity, object>> expression = e => new { /* projection */ };

    // Act - Parse the expression and generate OData query parts
    var node = visitor.Parse(expression);
    var result = node.GetSelectExpand(ODataVersion.V4);

    // Assert - Verify the generated $select and $expand strings
    Assert.Equal("expected-select", result.select);
    Assert.Equal("expected-expand", result.expand);
}
```

## Related Files

- **Implementation:**
  - `src\Linq2OData.Core\Expressions\SelectExpressionVisitor.cs` - Parses LINQ expressions
  - `src\Linq2OData.Core\Expressions\QueryNode.cs` - Builds OData query strings (key fix location)
  - `src\Linq2OData.Core\Builders\GetProjectionBuilder.cs` - Orchestrates projection building

- **Bug Fix:**
  - The critical fix was in `QueryNode.BuildExpand()` method (lines 78-108)
  - Changed from: `parts.Add(selectedProps)` and `parts.Add(string.Join(",", nestedExpands))`
  - Changed to: `parts.Add($"$select={selectedProps}")` and `parts.Add($"$expand={string.Join(",", nestedExpands)}")`

## Integration Testing

These unit tests focus on the expression parsing and query generation. For end-to-end integration testing with actual OData services, see:
- `test\Linq2OData.TestClients\Program.cs` - Real-world OData service calls
- `ODataClientIntegrationTests.cs` - Integration test suite

## Test Coverage Summary

| Category | V4 Tests | V2 Tests | Total |
|----------|----------|----------|-------|
| Simple Properties | 2 | 1 | 3 |
| Simple Expand | 1 | 1 | 2 |
| Nested Expand | 4 | 1 | 5 |
| Complex Scenarios | 3 | 0 | 3 |
| Edge Cases | 3 | 1 | 4 |
| **Total** | **13** | **4** | **17** |

All tests are passing ✅
