# Expand Function Test Suite

## Overview
Created a comprehensive test suite for the OData `$expand` functionality in the file `test\Linq2OData.Tests\ExpandTests.cs`.

## Test Coverage Summary

### Total Tests: 28
All tests are passing ✅

## Test Categories

### 1. Simple Expand Tests (4 tests)
Tests basic expand scenarios for single navigation properties:
- `Expand_SingleNavigationProperty_V4_GeneratesCorrectExpand`
- `Expand_SingleNavigationProperty_V2_GeneratesCorrectExpand`
- `Expand_CollectionNavigationProperty_V4_GeneratesCorrectExpand`
- `Expand_OnlyNavigationProperty_V4_NoSelectGenerated`

**Key Scenarios:**
- Single navigation property expansion (V4 and V2)
- Collection navigation property expansion
- Expansion without simple properties (only complex properties)

### 2. Multiple Root Level Expands (3 tests)
Tests expanding multiple navigation properties at the root level:
- `Expand_MultipleNavigationProperties_V4_GeneratesCommaSeparatedExpands`
- `Expand_MultipleNavigationProperties_V2_GeneratesCommaSeparatedExpands`
- `Expand_ThreeNavigationProperties_V4_GeneratesCorrectExpand`

**Key Scenarios:**
- Multiple navigation properties with comma separation
- Both V2 and V4 format validation

### 3. Nested Expand Tests (5 tests)
Tests deep navigation property chains:
- `Expand_TwoLevelNested_V4_GeneratesCorrectNestedExpand`
- `Expand_TwoLevelNested_V2_GeneratesSlashNotation`
- `Expand_ThreeLevelNested_V4_GeneratesDeepNestedExpand`
- `Expand_FourLevelNested_V4_GeneratesVeryDeepNestedExpand`
- `Expand_FourLevelNested_V2_GeneratesSlashNotation`

**Key Scenarios:**
- 2-level, 3-level, and 4-level nested expands
- V4 format: `Customer($expand=Address($expand=Country))`
- V2 format: `Customer/Address/Country` (slash notation)

### 4. Nested Expand with Properties (3 tests)
Tests nested expands with property selection at each level:
- `Expand_NestedWithPropertySelection_V4_GeneratesExpandWithSelect`
- `Expand_DeepNestedWithPropertyAtEachLevel_V4_GeneratesCorrectQuery`
- `Expand_NestedWithMultiplePropertiesAtSameLevel_V4_GeneratesCorrectSelect`

**Key Scenarios:**
- Combining `$select` with `$expand` at nested levels
- Format: `Customer($select=Name;$expand=Address($select=City))`
- Multiple simple properties at the same level

### 5. Multiple Branches in Expand (3 tests)
Tests complex expand trees with multiple branches:
- `Expand_MultipleBranchesFromSameEntity_V4_GeneratesCorrectExpand`
- `Expand_MultipleDifferentDepthBranches_V4_GeneratesCorrectExpand`
- `Expand_TwoDifferentRootLevelWithNestedExpands_V4_GeneratesCorrectExpand`

**Key Scenarios:**
- Multiple navigation properties from the same parent
- Different depth levels in the same query
- Complex expand trees with multiple root-level and nested expands

### 6. Self-Referencing Expand (2 tests)
Tests entities that reference themselves (e.g., Category -> ParentCategory):
- `Expand_SelfReferencingEntity_V4_GeneratesCorrectExpand`
- `Expand_SelfReferencingEntityNested_V4_GeneratesCorrectExpand`

**Key Scenarios:**
- Simple self-reference
- Nested self-reference (e.g., Category -> ParentCategory -> ParentCategory)

### 7. Complex Real-World Scenarios (3 tests)
Tests realistic business scenarios:
- `Expand_ComplexOrderScenario_V4_GeneratesCorrectExpand`
  - Order with customer details, address, and order items
- `Expand_ComplexProductScenario_V4_GeneratesCorrectExpand`
  - Product with category hierarchy and supplier location
- `Expand_MixedSimpleAndComplexExpands_V4_GeneratesCorrectExpand`
  - Mixed simple and nested expands in one query

**Key Scenarios:**
- Real-world e-commerce queries
- Multiple navigation properties with different depths
- Complex select and expand combinations

### 8. Edge Cases (3 tests)
Tests boundary conditions and special cases:
- `Expand_OnlyComplexProperties_V4_OnlyExpandGenerated`
- `Expand_OnlyDeepNestedProperty_V4_GeneratesOnlyExpand`
- `Expand_MultiplePropertiesFromSameNestedEntity_V4_GroupsCorrectly`

**Key Scenarios:**
- No simple properties, only expands
- Very deep navigation without root-level properties
- Multiple properties from the same nested entity

### 9. OData Version Comparison Tests (2 tests)
Tests consistency across OData versions:
- `Expand_SameQuery_V2AndV4_GeneratesDifferentFormats`
- `Expand_ComplexNestedQuery_V3_GeneratesSlashNotation`

**Key Scenarios:**
- Comparing V2 and V4 output for the same query
- V3 format validation
- Format differences: nested vs. slash notation

## Test Entities

The test suite uses realistic domain entities:
- **TestOrder**: Orders with customers and order items
- **TestCustomer**: Customers with addresses, orders, and contacts
- **TestAddress**: Addresses with country references
- **TestCountry**: Countries with region references
- **TestProduct**: Products with categories and suppliers
- **TestCategory**: Categories with self-referencing parent categories
- **TestSupplier**: Suppliers with addresses
- **TestOrderItem**: Order line items with products
- **TestContact**: Contact information

## OData Version Support

The tests cover all three major OData versions:
- **OData V2**: Uses slash notation for nested expands
  - Example: `$expand=Customer/Address/Country`
- **OData V3**: Uses slash notation (similar to V2)
- **OData V4**: Uses nested parentheses with `$expand=` prefix
  - Example: `$expand=Customer($expand=Address($expand=Country))`

## Test Results

✅ **All 28 tests pass**
✅ **All 165 total project tests pass**
✅ **No regressions introduced**

## Key Testing Patterns

1. **Arrange-Act-Assert** pattern consistently used
2. **Clear test names** that describe the scenario and expected outcome
3. **Version-specific tests** to ensure compatibility
4. **Progressive complexity** from simple to complex scenarios
5. **Comments** explaining the business context of each test

## Coverage Gaps Addressed

The new test suite fills the following gaps in the existing `SelectProjectionTests.cs`:
1. **Dedicated expand focus**: While SelectProjectionTests covers select+expand, this suite focuses purely on expand scenarios
2. **More complex nesting**: Tests up to 4 levels deep
3. **Self-referencing entities**: A common real-world scenario
4. **Multiple branch expansion**: Complex tree structures
5. **Real-world scenarios**: E-commerce-style queries with orders, customers, and products

## Running the Tests

Run all expand tests:
```bash
dotnet test --filter "FullyQualifiedName~Linq2OData.Tests.ExpandTests"
```

Run a specific test:
```bash
dotnet test --filter "FullyQualifiedName~Expand_ComplexOrderScenario_V4_GeneratesCorrectExpand"
```

Run all tests in the project:
```bash
dotnet test
```

## Next Steps / Potential Enhancements

Consider adding tests for:
1. **Filter in expand**: `$expand=Orders($filter=Status eq 'Active')`
2. **OrderBy in expand**: `$expand=Orders($orderby=OrderDate desc)`
3. **Top/Skip in expand**: `$expand=Orders($top=5)`
4. **Select in expand**: `$expand=Orders($select=ID,OrderDate)`
5. **Count in expand**: `$expand=Orders/$count`
6. **Expand with lambda expressions**: More complex LINQ scenarios
7. **Error cases**: Invalid navigation paths, null references
8. **Performance tests**: Large expand trees

## Summary

The `ExpandTests.cs` file provides comprehensive coverage of the OData expand functionality, ensuring that both simple and complex expand scenarios work correctly across different OData versions. The tests are well-organized, clearly named, and cover realistic business scenarios.
