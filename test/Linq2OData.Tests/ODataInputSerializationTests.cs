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
    /// Tests that null navigation properties are handled correctly for OData V2.
    /// </summary>
    [Fact]
    public void ODataV2_SerializeInput_NullNavigationProperties_ShouldSerializeAsNull()
    {
        // Arrange
        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V2);
        var input = new TestProductInput
        {
            Name = "Test Product",
            Price = 99.99m,
            Category = null,
            Tags = null
        };

        // Act
        var json = JsonSerializer.Serialize(input.GetValues(), odataClient.JsonOptions);
        var jsonDoc = JsonDocument.Parse(json);

        // Assert
        Assert.Equal("Test Product", jsonDoc.RootElement.GetProperty("Name").GetString());
        
        // Null properties should not be present in the serialized JSON
        // (ODataInputBase.SetValue doesn't add null values)
        Assert.False(jsonDoc.RootElement.TryGetProperty("Category", out _));
        Assert.False(jsonDoc.RootElement.TryGetProperty("Tags", out _));
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
    }

    private class TestCategoryInput
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
    }

    private class TestTagInput
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = "";
    }
}
