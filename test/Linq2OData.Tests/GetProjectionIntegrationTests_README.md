# GetProjectionIntegrationTests

This test suite provides **integration-level testing** for the projection (Select) functionality when using the `GetBuilder` API for retrieving single entities by key.

## Purpose

These tests validate end-to-end projection behavior when using the `.Get()` method combined with `.Select()`:

```csharp
var person = await client
    .Get<Person>(p => p.ID = 4)
    .Select(p => new { p.Name, PersonDetail = p.PersonDetail!.Person })
    .ExecuteAsync();
```

The tests ensure that:
- The `GetBuilder<T>` correctly processes projection expressions
- Internal `$select` and `$expand` fields are set correctly
- The resulting OData URL would be properly formatted
- Both OData v2/v3 and v4 syntax are handled correctly

## Difference from SelectProjectionTests

| Aspect | SelectProjectionTests | GetProjectionIntegrationTests |
|--------|----------------------|------------------------------|
| **Level** | Unit tests | Integration tests |
| **Focus** | Expression parsing & query generation | End-to-end GetBuilder API |
| **Tests** | `QueryNodeVisitor` and `QueryNode` | `GetBuilder<T>` with `.Select()` |
| **Method** | Direct expression parsing | Full builder API flow |
| **Validation** | Query string generation | Internal builder state |

## Test Scenarios

### 1. Nested Projection with Get (Original Bug Scenario)
**Test:** `GetBuilder_WithNestedProjection_GeneratesCorrectODataUrl_V4`

The critical integration test that validates the bug fix for nested projections:

```csharp
// This was generating incorrect OData in the bug report
var projectionBuilder = getBuilder.Select(e => new 
{ 
    e.Name, 
    e.ID, 
    PersonDetail = e.PersonDetail!.Person 
});

// After fix, generates correct OData v4 syntax:
// Persons(ID=4)?$select=Name,ID&$expand=PersonDetail($expand=Person)
```

**Validates:**
- ✅ `select` field: `"Name,ID"`
- ✅ `expand` field: `"PersonDetail($expand=Person)"`

### 2. Complex Nested Projection
**Test:** `GetBuilder_WithComplexNestedProjection_GeneratesCorrectQuery_V4`

Tests projecting multiple properties from a nested navigation object:

```csharp
var projectionBuilder = getBuilder.Select(pd => new 
{ 
    pd.Age,
    pd.Phone,
    PersonName = pd.Person!.Name,
    PersonEmail = pd.Person!.Email
});

// Generates: $select=Age,Phone&$expand=Person($select=Name,Email)
```

**Validates:**
- ✅ Root properties in `$select`
- ✅ Nested properties in `$expand` with nested `$select`

### 3. Simple Projection
**Test:** `GetBuilder_WithSimpleProjection_GeneratesCorrectQuery_V4`

Tests basic property selection without navigation:

```csharp
var projectionBuilder = getBuilder.Select(e => new { e.Name, e.Email });

// Generates: $select=Name,Email (no expand)
```

### 4. Only Nested Object
**Test:** `GetBuilder_WithOnlyNestedObject_GeneratesCorrectQuery_V4`

Tests projecting only a navigation property:

```csharp
var projectionBuilder = getBuilder.Select(e => new { e.PersonDetail });

// Generates: $expand=PersonDetail (empty select in v4)
```

**Note:** OData v4 allows empty `$select` when only expanding.

### 5. OData V2 Compatibility
**Test:** `GetBuilder_V2_WithNestedProjection_GeneratesCorrectQuery`

Validates that OData v2/v3 uses different syntax:

```csharp
var projectionBuilder = getBuilder.Select(e => new 
{ 
    e.Name, 
    PersonDetail = e.PersonDetail!.Person 
});

// OData v2 generates: $select=Name,PersonDetail&$expand=PersonDetail/Person
```

**Key Differences:**
- V2 includes complex properties in `$select`
- V2 uses slash notation (`/`) instead of nested parentheses

## Test Implementation Details

### Reflection-Based Validation

Since `SetProjection()` is an internal method, tests use reflection to invoke it:

```csharp
var setProjectionMethod = projectionBuilder.GetType()
    .GetMethod("SetProjection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
setProjectionMethod?.Invoke(projectionBuilder, null);
```

Then validate internal fields using reflection:

```csharp
var selectProperty = typeof(GetBuilder<TestPerson>)
    .GetField("select", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
var select = selectProperty?.GetValue(getBuilder) as string;

Assert.Equal("Name,ID", select);
```

### Why Integration Tests Matter

1. **API Flow Validation** - Tests the actual user-facing API, not just internal components
2. **State Verification** - Ensures internal builder state is correct after projections
3. **Real-World Scenarios** - Uses the same code patterns users would write
4. **Regression Prevention** - Catches issues in the complete flow, not just isolated components

## Running the Tests

Run all GetProjectionIntegrationTests:

```bash
dotnet test --filter "FullyQualifiedName~GetProjectionIntegrationTests"
```

Run a specific test:

```bash
dotnet test --filter "FullyQualifiedName~GetBuilder_WithNestedProjection_GeneratesCorrectODataUrl_V4"
```

Run only OData v4 tests:

```bash
dotnet test --filter "FullyQualifiedName~GetProjectionIntegrationTests&FullyQualifiedName~V4"
```

## Test Coverage Summary

| Scenario | OData v4 | OData v2 | Total |
|----------|----------|----------|-------|
| Nested Projection | ✅ | ✅ | 2 |
| Complex Nested | ✅ | - | 1 |
| Simple Projection | ✅ | - | 1 |
| Only Nested Object | ✅ | - | 1 |
| **Total** | **5** | **1** | **6** |

All tests passing ✅

## Related Files

### Test Files
- **`SelectProjectionTests.cs`** - Unit tests for expression parsing
- **`SelectProjectionTests_README.md`** - Documentation for unit tests
- **This file** - Integration test documentation

### Implementation Files
- **`src/Linq2OData.Core/Builders/GetBuilder.cs`** - Handles single entity retrieval by key
- **`src/Linq2OData.Core/Builders/GetProjectionBuilder.cs`** - Projection builder for Get operations
- **`src/Linq2OData.Core/Builders/QueryProjectionBuilder.cs`** - Projection builder for Query operations
- **`src/Linq2OData.Core/Expressions/QueryNode.cs`** - Core query generation logic (line 78-108 contains the critical fix)

## The Bug That Was Fixed

**Before the fix:**
```
Persons(ID=4)?$select=Name,ID&$expand=PersonDetail(Person)  ❌ Invalid OData v4
```

**After the fix:**
```
Persons(ID=4)?$select=Name,ID&$expand=PersonDetail($expand=Person)  ✅ Valid OData v4
```

The fix was in `QueryNode.BuildExpand()` which now correctly adds the `$expand=` prefix for nested navigation properties in OData v4.

## Example Usage in Production

```csharp
// Initialize client
var httpClient = new HttpClient 
{ 
    BaseAddress = new Uri("https://api.example.com/odata/") 
};
var client = new MyODataClient(httpClient, ODataVersion.V4);

// Get a single person with projection
var person = await client
    .Get<Person>(p => p.ID = 4)
    .Select(p => new 
    { 
        p.Name, 
        p.ID, 
        PersonDetail = p.PersonDetail!.Person 
    })
    .ExecuteAsync();

// Result: Only Name, ID, and nested Person are retrieved
// URL: Persons(ID=4)?$select=Name,ID&$expand=PersonDetail($expand=Person)
```

## Benefits of Integration Testing

- ✅ **Catches API-level issues** that unit tests might miss
- ✅ **Validates the complete flow** from user code to query generation
- ✅ **Tests real-world usage patterns** that developers actually use
- ✅ **Provides confidence** in refactoring internal implementations
- ✅ **Documents expected behavior** for the public API

## Future Test Additions

Potential areas for additional integration test coverage:

- [ ] Multiple nested levels (3+ deep)
- [ ] Combining projection with `.Filter()` and `.Expand()`
- [ ] Collection navigation properties in projections
- [ ] Null handling and error cases
- [ ] Performance benchmarks for complex projections
