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

1. **`ODataCollectionConverter<T>`** (`src\Linq2OData.Core\Converters\ODataCollectionConverter.cs`)
   - Handles serialization/deserialization of collection navigation properties
   - **Read**: Unwraps `{"results": [...]}` format or handles `{"__deferred": {...}}`
   - **Write**: Wraps collections in `{"results": [...]}` for V2/V3

2. **`ODataCollectionConverterFactory`** (`src\Linq2OData.Core\Converters\ODataCollectionConverterFactory.cs`)
   - Creates `ODataCollectionConverter<T>` instances for `List<T>` types
   - Only registered for OData versions < V4

3. **`ODataNavigationPropertyConverter<T>`** (`src\Linq2OData.Core\Converters\ODataNavigationPropertyConverter.cs`)
   - Handles serialization/deserialization of single navigation properties
   - **Read**: Handles `{"__deferred": {...}}` or expanded objects
   - **Write**: Serializes as plain objects (correct for both V2/V3 and V4)

4. **`ODataNavigationPropertyConverterFactory`** (`src\Linq2OData.Core\Converters\ODataNavigationPropertyConverterFactory.cs`)
   - Creates `ODataNavigationPropertyConverter<T>` instances for entity types
   - Only registered for OData versions < V4

### Configuration

The converters are registered in `ODataClient` constructor based on the OData version:

```csharp
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
```

For OData V4, no special converters are added, and the default JSON serialization is used.

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

## Testing

Comprehensive tests are provided in `test\Linq2OData.Tests\ODataInputSerializationTests.cs`:

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

## Compliance

This implementation follows the official OData specifications:
- [OData Version 2.0](http://www.odata.org/documentation/odata-version-2-0/)
- [OData Version 3.0](http://www.odata.org/documentation/odata-version-3-0/)
- [OData Version 4.0](http://docs.oasis-open.org/odata/odata/v4.0/odata-v4.0-part1-protocol.html)
