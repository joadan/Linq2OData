# OData Navigation Property Serialization for Create/Update Operations

## Overview

This document describes how navigation properties are serialized when creating or updating entities using `ODataInputBase`-derived classes in OData V2/V3 vs V4.

## OData Version Differences

### OData V2/V3 (OData versions < 4)

According to the OData V2/V3 specification:

- **Collection Navigation Properties** (one-to-many relationships): Must be wrapped in a `"results"` object
- **Single Navigation Properties** (one-to-one, many-to-one relationships): Serialized as plain objects (no wrapper)

#### Example V2/V3 JSON:

```json
{
  "Name": "Product A",
  "Price": 100.00,
  "Category": {
    "CategoryId": 5,
    "CategoryName": "Electronics"
  },
  "Tags": {
    "results": [
      {"TagId": 1, "TagName": "Popular"},
      {"TagId": 2, "TagName": "New"}
    ]
  }
}
```

### OData V4

In OData V4, both single and collection navigation properties are serialized without wrappers:

#### Example V4 JSON:

```json
{
  "Name": "Product A",
  "Price": 100.00,
  "Category": {
    "CategoryId": 5,
    "CategoryName": "Electronics"
  },
  "Tags": [
    {"TagId": 1, "TagName": "Popular"},
    {"TagId": 2, "TagName": "New"}
  ]
}
```

## Implementation

### Components

1. **`ODataInputBaseConverter`** (`src\Linq2OData.Core\Converters\ODataInputBaseConverter.cs`)
   - Handles serialization of `ODataInputBase` objects by calling `GetValues()` recursively
   - Ensures nested `ODataInputBase` objects (e.g., Input classes within Input classes) are properly serialized
   - Only supports write operations (create/update), not deserialization

2. **`ODataCollectionConverter<T>`** (`src\Linq2OData.Core\Converters\ODataCollectionConverter.cs`)
   - Handles serialization/deserialization of collection navigation properties
   - **Read**: Unwraps `{"results": [...]}` format or handles `{"__deferred": {...}}`
   - **Write**: Wraps collections in `{"results": [...]}` for V2/V3

3. **`ODataCollectionConverterFactory`** (`src\Linq2OData.Core\Converters\ODataCollectionConverterFactory.cs`)
   - Creates `ODataCollectionConverter<T>` instances for `List<T>` types
   - Only registered for OData versions < V4

4. **`ODataNavigationPropertyConverter<T>`** (`src\Linq2OData.Core\Converters\ODataNavigationPropertyConverter.cs`)
   - Handles serialization/deserialization of single navigation properties
   - **Read**: Handles `{"__deferred": {...}}` or expanded objects
   - **Write**: Serializes as plain objects (correct for both V2/V3 and V4)

5. **`ODataNavigationPropertyConverterFactory`** (`src\Linq2OData.Core\Converters\ODataNavigationPropertyConverterFactory.cs`)
   - Creates `ODataNavigationPropertyConverter<T>` instances for entity types
   - Only registered for OData versions < V4

### Configuration

The converters are registered in `ODataClient` constructor based on the OData version:

```csharp
public ODataClient(HttpClient httpClient, ODataVersion odataVersion)
{
    // ...
    jsonOptions = new JsonSerializerOptions();

    // Add ODataInputBase converter for all versions to handle nested Input objects
    jsonOptions.Converters.Add(new ODataInputBaseConverter());

    if (odataVersion < ODataVersion.V4)
    {
        jsonOptions.Converters.Add(new MicrosoftDateTimeConverter());
        jsonOptions.Converters.Add(new MicrosoftNullableDateTimeConverter());
        jsonOptions.Converters.Add(new MicrosoftDateTimeOffsetConverter());
        jsonOptions.Converters.Add(new DecimalStringJsonConverter());
        jsonOptions.Converters.Add(new Int64StringJsonConverter());
        jsonOptions.Converters.Add(new NullableInt64StringJsonConverter());
        // Add the collection converter for handling "results" wrapper
        jsonOptions.Converters.Add(new ODataCollectionConverterFactory());
        // Add the navigation property converter for handling "__deferred" wrapper
        jsonOptions.Converters.Add(new ODataNavigationPropertyConverterFactory());
    }
}
```

**Key Points:**
- `ODataInputBaseConverter` is registered for **all** OData versions to handle nested `ODataInputBase` objects
- Version-specific converters (collection and navigation property converters) are only registered for V2/V3
- For OData V4, only the `ODataInputBaseConverter` is used, and default JSON serialization applies

## Usage

When creating or updating entities with navigation properties:

```csharp
// Create a product with navigation properties
var productInput = new ProductInput
{
    Name = "Test Product",
    Price = 99.99m,
    Category = new CategoryInput  // Single navigation property
    {
        CategoryId = 5,
        CategoryName = "Electronics"
    },
    Tags = new List<TagInput>  // Collection navigation property
    {
        new TagInput { TagId = 1, TagName = "Popular" },
        new TagInput { TagId = 2, TagName = "New" }
    }
};

// Create entity - serialization is handled automatically based on OData version
var createdProduct = await client.CreateEntityAsync<Product>("Products", productInput);

// Update entity - same automatic handling
await client.UpdateEntityAsync("Products", "123", productInput);
```

### Nested ODataInputBase Objects

The implementation fully supports nested `ODataInputBase` objects, which is common when Input classes reference other Input classes:

```csharp
// Example: Order with nested Product, which has nested Category
var orderInput = new OrderInput
{
    OrderId = 123,
    Product = new ProductInput  // Nested ODataInputBase
    {
        Name = "Laptop",
        Price = 999.99m,
        Category = new CategoryInput  // Deeply nested ODataInputBase
        {
            CategoryId = 5,
            CategoryName = "Electronics"
        },
        Tags = new List<TagInput>  // Collection in nested object
        {
            new TagInput { TagId = 1, TagName = "Premium" }
        }
    }
};

// The ODataInputBaseConverter ensures all nested Input objects
// are properly serialized by calling GetValues() recursively
var createdOrder = await client.CreateEntityAsync<Order>("Orders", orderInput);
```

**For OData V2/V3, this produces:**
```json
{
  "OrderId": 123,
  "Product": {
    "Name": "Laptop",
    "Price": "999.99",
    "Category": {
      "CategoryId": 5,
      "CategoryName": "Electronics"
    },
    "Tags": {
      "results": [
        {"TagId": 1, "TagName": "Premium"}
      ]
    }
  }
}
```

**For OData V4, this produces:**
```json
{
  "OrderId": 123,
  "Product": {
    "Name": "Laptop",
    "Price": 999.99,
    "Category": {
      "CategoryId": 5,
      "CategoryName": "Electronics"
    },
    "Tags": [
      {"TagId": 1, "TagName": "Premium"}
    ]
  }
}
```

## Testing

Comprehensive tests are provided in `test\Linq2OData.Tests\ODataInputSerializationTests.cs`:

### Basic Tests

1. **`ODataV2_SerializeInput_CollectionNavigationProperty_ShouldWrapInResults`**
   - Verifies collection navigation properties are wrapped in "results" for V2

2. **`ODataV2_SerializeInput_SingleNavigationProperty_ShouldNotWrap`**
   - Verifies single navigation properties are NOT wrapped for V2

3. **`ODataV2_SerializeInput_MixedNavigationProperties_ShouldHandleCorrectly`**
   - Verifies both types of navigation properties work together correctly

4. **`ODataV4_SerializeInput_CollectionNavigationProperty_ShouldNotWrap`**
   - Verifies V4 doesn't wrap collections (no converters applied)

5. **`ODataV2_SerializeInput_NullNavigationProperties_ShouldSerializeAsNull`**
   - Verifies null navigation properties are handled correctly

6. **`ODataV2_SerializeInput_EmptyCollectionNavigationProperty_ShouldWrapInResults`**
   - Verifies empty collections are still wrapped in "results"

### Nested ODataInputBase Tests

7. **`ODataV2_SerializeInput_NestedODataInputBase_ShouldCallGetValuesRecursively`**
   - Verifies nested `ODataInputBase` objects are serialized by calling `GetValues()` recursively

8. **`ODataV2_SerializeInput_DeeplyNestedODataInputBase_ShouldSerializeCorrectly`**
   - Tests deeply nested structures (3+ levels of `ODataInputBase` objects)

9. **`ODataV2_SerializeInput_NestedODataInputBaseWithCollections_ShouldWrapCollections`**
   - Verifies that collections within nested `ODataInputBase` objects are properly wrapped

10. **`ODataV4_SerializeInput_NestedODataInputBase_ShouldSerializeWithoutWrappers`**
    - Verifies V4 correctly handles nested objects without wrappers

## Compliance

This implementation follows the official OData specifications:
- [OData Version 2.0](http://www.odata.org/documentation/odata-version-2-0/)
- [OData Version 3.0](http://www.odata.org/documentation/odata-version-3-0/)
- [OData Version 4.0](http://docs.oasis-open.org/odata/odata/v4.0/odata-v4.0-part1-protocol.html)
