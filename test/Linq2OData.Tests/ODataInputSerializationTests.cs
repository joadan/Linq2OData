using Linq2OData.Core;
using System.Globalization;
using System.Text.Json;

namespace Linq2OData.Tests;

/// <summary>
/// Tests for verifying correct serialization of ODataInputBase (used in create/update operations)
/// with navigation properties for different OData versions.
/// </summary>
public class ODataInputSerializationTests
{
    /// <summary>
    /// Tests that collection navigation properties are wrapped in "results" for OData V2
    /// when serializing ODataInputBase for create/update operations.
    /// </summary>
    [Fact]
    public void ODataV2_SerializeInput_CollectionNavigationProperty_ShouldWrapInResults()
    {
        // Arrange
        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V2);
        var input = new TestProductInput
        {
            Name = "Test Product",
            Price = 99.99m,
            Tags = new List<TestTagInput>
            {
                new TestTagInput { TagId = 1, TagName = "Popular" },
                new TestTagInput { TagId = 2, TagName = "New" }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(input.GetValues(), odataClient.JsonOptions);
        var jsonDoc = JsonDocument.Parse(json);

        // Assert
        Assert.Equal("Test Product", jsonDoc.RootElement.GetProperty("Name").GetString());
        // Decimal might be serialized as string in V2
        var priceElement = jsonDoc.RootElement.GetProperty("Price");
        var priceValue = priceElement.ValueKind == JsonValueKind.String 
            ? decimal.Parse(priceElement.GetString()!, CultureInfo.InvariantCulture) 
            : priceElement.GetDecimal();
        Assert.Equal(99.99m, priceValue);
        
        // Collection navigation property should be wrapped in "results"
        var tagsProperty = jsonDoc.RootElement.GetProperty("Tags");
        Assert.Equal(JsonValueKind.Object, tagsProperty.ValueKind);
        Assert.True(tagsProperty.TryGetProperty("results", out var results));
        Assert.Equal(JsonValueKind.Array, results.ValueKind);
        Assert.Equal(2, results.GetArrayLength());
        
        var firstTag = results[0];
        Assert.Equal(1, firstTag.GetProperty("TagId").GetInt32());
        Assert.Equal("Popular", firstTag.GetProperty("TagName").GetString());
    }

    /// <summary>
    /// Tests that single navigation properties are NOT wrapped for OData V2
    /// when serializing ODataInputBase for create/update operations.
    /// </summary>
    [Fact]
    public void ODataV2_SerializeInput_SingleNavigationProperty_ShouldNotWrap()
    {
        // Arrange
        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V2);
        var input = new TestProductInput
        {
            Name = "Test Product",
            Price = 99.99m,
            Category = new TestCategoryInput
            {
                CategoryId = 5,
                CategoryName = "Electronics"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(input.GetValues(), odataClient.JsonOptions);
        var jsonDoc = JsonDocument.Parse(json);

        // Assert
        Assert.Equal("Test Product", jsonDoc.RootElement.GetProperty("Name").GetString());

        // Single navigation property should be a plain object (not wrapped)
        var categoryProperty = jsonDoc.RootElement.GetProperty("Category");
        Assert.Equal(JsonValueKind.Object, categoryProperty.ValueKind);
        
        // Should NOT have "results" wrapper
        Assert.False(categoryProperty.TryGetProperty("results", out _));
        
        // Should have direct properties
        Assert.Equal(5, categoryProperty.GetProperty("CategoryId").GetInt32());
        Assert.Equal("Electronics", categoryProperty.GetProperty("CategoryName").GetString());
    }

    /// <summary>
    /// Tests that both single and collection navigation properties are handled correctly
    /// in the same input object for OData V2.
    /// </summary>
    [Fact]
    public void ODataV2_SerializeInput_MixedNavigationProperties_ShouldHandleCorrectly()
    {
        // Arrange
        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V2);
        var input = new TestProductInput
        {
            Name = "Test Product",
            Price = 99.99m,
            Category = new TestCategoryInput
            {
                CategoryId = 5,
                CategoryName = "Electronics"
            },
            Tags = new List<TestTagInput>
            {
                new TestTagInput { TagId = 1, TagName = "Popular" },
                new TestTagInput { TagId = 2, TagName = "New" }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(input.GetValues(), odataClient.JsonOptions);
        var jsonDoc = JsonDocument.Parse(json);

        // Assert
        // Single navigation property should NOT be wrapped
        var categoryProperty = jsonDoc.RootElement.GetProperty("Category");
        Assert.Equal(JsonValueKind.Object, categoryProperty.ValueKind);
        Assert.False(categoryProperty.TryGetProperty("results", out _));
        Assert.Equal(5, categoryProperty.GetProperty("CategoryId").GetInt32());
        
        // Collection navigation property should be wrapped in "results"
        var tagsProperty = jsonDoc.RootElement.GetProperty("Tags");
        Assert.Equal(JsonValueKind.Object, tagsProperty.ValueKind);
        Assert.True(tagsProperty.TryGetProperty("results", out var results));
        Assert.Equal(JsonValueKind.Array, results.ValueKind);
        Assert.Equal(2, results.GetArrayLength());
    }

    /// <summary>
    /// Tests that OData V4 does NOT wrap collections in "results" 
    /// (converters are not used for V4).
    /// </summary>
    [Fact]
    public void ODataV4_SerializeInput_CollectionNavigationProperty_ShouldNotWrap()
    {
        // Arrange
        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V4);
        var input = new TestProductInput
        {
            Name = "Test Product",
            Price = 99.99m,
            Tags = new List<TestTagInput>
            {
                new TestTagInput { TagId = 1, TagName = "Popular" },
                new TestTagInput { TagId = 2, TagName = "New" }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(input.GetValues(), odataClient.JsonOptions);
        var jsonDoc = JsonDocument.Parse(json);

        // Assert
        Assert.Equal("Test Product", jsonDoc.RootElement.GetProperty("Name").GetString());

        // In V4, collection should be a plain array (no "results" wrapper)
        var tagsProperty = jsonDoc.RootElement.GetProperty("Tags");
        Assert.Equal(JsonValueKind.Array, tagsProperty.ValueKind);
        Assert.Equal(2, tagsProperty.GetArrayLength());
        
        var firstTag = tagsProperty[0];
        Assert.Equal(1, firstTag.GetProperty("TagId").GetInt32());
        Assert.Equal("Popular", firstTag.GetProperty("TagName").GetString());
    }

    /// <summary>
    /// Tests that explicitly set null navigation properties are serialized as null for OData V2.
    /// This is important for UPDATE operations where you want to clear a value.
    /// </summary>
    [Fact]
    public void ODataV2_SerializeInput_ExplicitlySetNullNavigationProperties_ShouldSerializeAsNull()
    {
        // Arrange
        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V2);
        var input = new TestProductInput
        {
            Name = "Test Product",
            Price = 99.99m,
            Category = null,  // Explicitly set to null
            Tags = null       // Explicitly set to null
        };

        // Act
        var json = JsonSerializer.Serialize(input.GetValues(), odataClient.JsonOptions);
        var jsonDoc = JsonDocument.Parse(json);

        // Assert
        Assert.Equal("Test Product", jsonDoc.RootElement.GetProperty("Name").GetString());

        // Explicitly set null properties SHOULD be present in the serialized JSON
        // This is important for UPDATE operations where you want to clear a value
        Assert.True(jsonDoc.RootElement.TryGetProperty("Category", out var categoryElement));
        Assert.Equal(JsonValueKind.Null, categoryElement.ValueKind);

        Assert.True(jsonDoc.RootElement.TryGetProperty("Tags", out var tagsElement));
        Assert.Equal(JsonValueKind.Null, tagsElement.ValueKind);
    }

    /// <summary>
    /// Tests that properties that are never set are NOT included in the serialized JSON.
    /// This demonstrates the difference between "not set" and "explicitly set to null".
    /// </summary>
    [Fact]
    public void ODataV2_SerializeInput_NeverSetProperties_ShouldNotBeInJson()
    {
        // Arrange
        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V2);
        var input = new TestProductInput
        {
            Name = "Test Product",
            Price = 99.99m
            // Category and Tags are never set (not even to null)
        };

        // Act
        var json = JsonSerializer.Serialize(input.GetValues(), odataClient.JsonOptions);
        var jsonDoc = JsonDocument.Parse(json);

        // Assert
        Assert.Equal("Test Product", jsonDoc.RootElement.GetProperty("Name").GetString());

        // Properties that were never set should NOT be in the JSON at all
        Assert.False(jsonDoc.RootElement.TryGetProperty("Category", out _));
        Assert.False(jsonDoc.RootElement.TryGetProperty("Tags", out _));
        Assert.False(jsonDoc.RootElement.TryGetProperty("CategoryInput", out _));
    }

    /// <summary>
    /// Tests that empty collection navigation properties are wrapped correctly for OData V2.
    /// </summary>
    [Fact]
    public void ODataV2_SerializeInput_EmptyCollectionNavigationProperty_ShouldWrapInResults()
    {
        // Arrange
        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V2);
        var input = new TestProductInput
        {
            Name = "Test Product",
            Price = 99.99m,
            Tags = new List<TestTagInput>() // Empty list
        };

        // Act
        var json = JsonSerializer.Serialize(input.GetValues(), odataClient.JsonOptions);
        var jsonDoc = JsonDocument.Parse(json);

        // Assert
        // Even empty collections should be wrapped in "results"
        var tagsProperty = jsonDoc.RootElement.GetProperty("Tags");
        Assert.Equal(JsonValueKind.Object, tagsProperty.ValueKind);
        Assert.True(tagsProperty.TryGetProperty("results", out var results));
        Assert.Equal(JsonValueKind.Array, results.ValueKind);
        Assert.Equal(0, results.GetArrayLength());
    }

    /// <summary>
    /// Tests that nested ODataInputBase objects are serialized correctly by calling GetValues() recursively.
    /// </summary>
    [Fact]
    public void ODataV2_SerializeInput_NestedODataInputBase_ShouldCallGetValuesRecursively()
    {
        // Arrange
        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V2);
        var input = new TestProductInput
        {
            Name = "Test Product",
            Price = 99.99m,
            CategoryInput = new TestCategoryODataInput // This is also ODataInputBase
            {
                CategoryId = 5,
                CategoryName = "Electronics"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(input.GetValues(), odataClient.JsonOptions);
        var jsonDoc = JsonDocument.Parse(json);

        // Assert
        Assert.Equal("Test Product", jsonDoc.RootElement.GetProperty("Name").GetString());

        // Nested ODataInputBase should be serialized as an object with its properties
        var categoryProperty = jsonDoc.RootElement.GetProperty("CategoryInput");
        Assert.Equal(JsonValueKind.Object, categoryProperty.ValueKind);
        Assert.Equal(5, categoryProperty.GetProperty("CategoryId").GetInt32());
        Assert.Equal("Electronics", categoryProperty.GetProperty("CategoryName").GetString());
    }

    /// <summary>
    /// Tests that deeply nested ODataInputBase objects are serialized correctly for OData V2.
    /// </summary>
    [Fact]
    public void ODataV2_SerializeInput_DeeplyNestedODataInputBase_ShouldSerializeCorrectly()
    {
        // Arrange
        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V2);
        var input = new TestOrderInput
        {
            OrderId = 123,
            Product = new TestProductInput
            {
                Name = "Test Product",
                Price = 99.99m,
                CategoryInput = new TestCategoryODataInput
                {
                    CategoryId = 5,
                    CategoryName = "Electronics"
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(input.GetValues(), odataClient.JsonOptions);
        var jsonDoc = JsonDocument.Parse(json);

        // Assert
        Assert.Equal(123, jsonDoc.RootElement.GetProperty("OrderId").GetInt32());

        var productProperty = jsonDoc.RootElement.GetProperty("Product");
        Assert.Equal(JsonValueKind.Object, productProperty.ValueKind);
        Assert.Equal("Test Product", productProperty.GetProperty("Name").GetString());

        var categoryProperty = productProperty.GetProperty("CategoryInput");
        Assert.Equal(JsonValueKind.Object, categoryProperty.ValueKind);
        Assert.Equal(5, categoryProperty.GetProperty("CategoryId").GetInt32());
        Assert.Equal("Electronics", categoryProperty.GetProperty("CategoryName").GetString());
    }

    /// <summary>
    /// Tests that nested ODataInputBase with collections works correctly for OData V2.
    /// </summary>
    [Fact]
    public void ODataV2_SerializeInput_NestedODataInputBaseWithCollections_ShouldWrapCollections()
    {
        // Arrange
        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V2);
        var input = new TestOrderInput
        {
            OrderId = 123,
            Product = new TestProductInput
            {
                Name = "Test Product",
                Tags = new List<TestTagInput>
                {
                    new TestTagInput { TagId = 1, TagName = "Popular" }
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(input.GetValues(), odataClient.JsonOptions);
        var jsonDoc = JsonDocument.Parse(json);

        // Assert
        var productProperty = jsonDoc.RootElement.GetProperty("Product");
        var tagsProperty = productProperty.GetProperty("Tags");

        // Collection should be wrapped in "results"
        Assert.Equal(JsonValueKind.Object, tagsProperty.ValueKind);
        Assert.True(tagsProperty.TryGetProperty("results", out var results));
        Assert.Equal(JsonValueKind.Array, results.ValueKind);
        Assert.Single(results.EnumerateArray());
    }

    /// <summary>
    /// Tests that nested ODataInputBase objects work correctly for OData V4 (no wrappers).
    /// </summary>
    [Fact]
    public void ODataV4_SerializeInput_NestedODataInputBase_ShouldSerializeWithoutWrappers()
    {
        // Arrange
        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V4);
        var input = new TestProductInput
        {
            Name = "Test Product",
            CategoryInput = new TestCategoryODataInput
            {
                CategoryId = 5,
                CategoryName = "Electronics"
            },
            Tags = new List<TestTagInput>
            {
                new TestTagInput { TagId = 1, TagName = "Popular" }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(input.GetValues(), odataClient.JsonOptions);
        var jsonDoc = JsonDocument.Parse(json);

        // Assert
        // Nested ODataInputBase should be a plain object
        var categoryProperty = jsonDoc.RootElement.GetProperty("CategoryInput");
        Assert.Equal(JsonValueKind.Object, categoryProperty.ValueKind);
        Assert.Equal(5, categoryProperty.GetProperty("CategoryId").GetInt32());

        // Collections should be plain arrays (no "results" wrapper in V4)
        var tagsProperty = jsonDoc.RootElement.GetProperty("Tags");
        Assert.Equal(JsonValueKind.Array, tagsProperty.ValueKind);
        Assert.Single(tagsProperty.EnumerateArray());
    }

    // Test input classes
    private class TestProductInput : ODataInputBase
    {
        public string? Name
        {
            get => GetValue<string?>(nameof(Name));
            set => SetValue(nameof(Name), value);
        }

        public decimal? Price
        {
            get => GetValue<decimal?>(nameof(Price));
            set => SetValue(nameof(Price), value);
        }

        public TestCategoryInput? Category
        {
            get => GetValue<TestCategoryInput?>(nameof(Category));
            set => SetValue(nameof(Category), value);
        }

        public List<TestTagInput>? Tags
        {
            get => GetValue<List<TestTagInput>?>(nameof(Tags));
            set => SetValue(nameof(Tags), value);
        }

        public TestCategoryODataInput? CategoryInput
        {
            get => GetValue<TestCategoryODataInput?>(nameof(CategoryInput));
            set => SetValue(nameof(CategoryInput), value);
        }
    }

    private class TestOrderInput : ODataInputBase
    {
        public int OrderId
        {
            get => GetValue<int>(nameof(OrderId));
            set => SetValue(nameof(OrderId), value);
        }

        public TestProductInput? Product
        {
            get => GetValue<TestProductInput?>(nameof(Product));
            set => SetValue(nameof(Product), value);
        }
    }

    private class TestCategoryInput
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
    }

    private class TestCategoryODataInput : ODataInputBase
    {
        public int CategoryId
        {
            get => GetValue<int>(nameof(CategoryId));
            set => SetValue(nameof(CategoryId), value);
        }

        public string? CategoryName
        {
            get => GetValue<string?>(nameof(CategoryName));
            set => SetValue(nameof(CategoryName), value);
        }
    }

    private class TestTagInput
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = "";
    }
}
