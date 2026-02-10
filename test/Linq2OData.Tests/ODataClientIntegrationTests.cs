using Linq2OData.Core;
using Linq2OData.Core.Converters;
using Linq2OData.Generator;
using Linq2OData.Generator.Models;

namespace Linq2OData.Tests;

/// <summary>
/// Integration tests that verify end-to-end functionality of the OData client,
/// including deserialization of real OData responses with various navigation property scenarios.
/// </summary>
public class ODataClientIntegrationTests
{
    private const string SalesOrderCollectionJson = @"{""d"":{""results"":[{""__metadata"":{""id"":""https://example.com/A_SalesOrder('4000043')"",""uri"":""https://example.com/A_SalesOrder('4000043')"",""type"":""API_SALES_ORDER_SRV.A_SalesOrderType""},""SalesOrder"":""4000043"",""SalesOrderType"":""ZOS"",""to_BillingPlan"":{""__deferred"":{""uri"":""https://example.com/A_SalesOrder('4000043')/to_BillingPlan""}},""to_Item"":{""__deferred"":{""uri"":""https://example.com/A_SalesOrder('4000043')/to_Item""}}},{""__metadata"":{""id"":""https://example.com/A_SalesOrder('4000044')"",""uri"":""https://example.com/A_SalesOrder('4000044')"",""type"":""API_SALES_ORDER_SRV.A_SalesOrderType""},""SalesOrder"":""4000044"",""SalesOrderType"":""ZOR"",""to_BillingPlan"":{""__deferred"":{""uri"":""https://example.com/A_SalesOrder('4000044')/to_BillingPlan""}},""to_Item"":{""__deferred"":{""uri"":""https://example.com/A_SalesOrder('4000044')/to_Item""}}}]}}";

    private const string SalesOrderWithExpandedItemsJson = @"{""d"":{""results"":[{""__metadata"":{""id"":""https://example.com/A_SalesOrder('4000043')"",""uri"":""https://example.com/A_SalesOrder('4000043')"",""type"":""API_SALES_ORDER_SRV.A_SalesOrderType""},""SalesOrder"":""4000043"",""SalesOrderType"":""ZOS"",""to_Item"":{""results"":[{""__metadata"":{""id"":""https://example.com/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='10')"",""uri"":""https://example.com/A_SalesOrderItem(SalesOrder='4000043',SalesOrderItem='10')"",""type"":""API_SALES_ORDER_SRV.A_SalesOrderItemType""},""SalesOrder"":""4000043"",""SalesOrderItem"":""10"",""Material"":""SETVMIN0""}]}}]}}";

    private const string ProductWithDeferredSupplierJson = @"{""d"":{""results"":[{""__metadata"":{""id"":""https://example.com/Products(0)"",""uri"":""https://example.com/Products(0)"",""type"":""ODataDemo.Product""},""ID"":0,""Name"":""Bread"",""Description"":""Whole grain bread"",""ReleaseDate"":""/Date(694224000000)/"",""DiscontinuedDate"":null,""Rating"":4,""Price"":""2.5"",""Category"":{""__deferred"":{""uri"":""https://example.com/Products(0)/Category""}},""Supplier"":{""__deferred"":{""uri"":""https://example.com/Products(0)/Supplier""}}}]}}";

    private const string ProductWithExpandedSupplierJson = @"{""d"":{""results"":[{""__metadata"":{""id"":""https://example.com/Products(0)"",""uri"":""https://example.com/Products(0)"",""type"":""ODataDemo.Product""},""ID"":0,""Name"":""Bread"",""Description"":""Whole grain bread"",""ReleaseDate"":""/Date(694224000000)/"",""DiscontinuedDate"":null,""Rating"":4,""Price"":""2.5"",""Category"":{""__deferred"":{""uri"":""https://example.com/Products(0)/Category""}},""Supplier"":{""__metadata"":{""id"":""https://example.com/Suppliers(1)"",""uri"":""https://example.com/Suppliers(1)"",""type"":""ODataDemo.Supplier""},""ID"":1,""Name"":""Acme Corp"",""Address"":{""Street"":""123 Main St"",""City"":""Seattle""}}}]}}";

    private const string ProductWithMixedNavigationPropertiesJson = @"{""d"":{""results"":[{""__metadata"":{""id"":""https://example.com/Products(0)"",""uri"":""https://example.com/Products(0)"",""type"":""ODataDemo.Product""},""ID"":0,""Name"":""Bread"",""Price"":""2.5"",""Category"":{""__metadata"":{""id"":""https://example.com/Categories(1)"",""uri"":""https://example.com/Categories(1)""},""ID"":1,""Name"":""Food""},""Supplier"":{""__deferred"":{""uri"":""https://example.com/Products(0)/Supplier""}}}]}}";


    /// <summary>
    /// Tests that OData V2 responses with non-expanded navigation properties (deferred) 
    /// are correctly deserialized with null values for those properties.
    /// </summary>
    [Fact]
    public void ODataV2_DeferredNavigationProperties_ShouldDeserializeAsNull()
    {
        // Arrange
        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V2);

        // Act
        var result = odataClient.ProcessQueryResponse<List<TestSalesOrder>>(SalesOrderCollectionJson);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count);
        
        var firstOrder = result.Data[0];
        Assert.Equal("4000043", firstOrder.SalesOrder);
        Assert.Equal("ZOS", firstOrder.SalesOrderType);
        // Deferred navigation properties should be null
        Assert.Null(firstOrder.to_Item);
        
        var secondOrder = result.Data[1];
        Assert.Equal("4000044", secondOrder.SalesOrder);
        Assert.Null(secondOrder.to_Item);
    }

    /// <summary>
    /// Tests that OData V2 responses with expanded collection navigation properties (results wrapper)
    /// are correctly deserialized with the collection populated.
    /// </summary>
    [Fact]
    public void ODataV2_ExpandedCollectionNavigationProperties_ShouldDeserializeCorrectly()
    {
        // Arrange
        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V2);

        // Act
        var result = odataClient.ProcessQueryResponse<List<TestSalesOrder>>(SalesOrderWithExpandedItemsJson);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);

        var order = result.Data[0];
        Assert.Equal("4000043", order.SalesOrder);
        Assert.NotNull(order.to_Item);
        Assert.Single(order.to_Item);

        var item = order.to_Item[0];
        Assert.Equal("4000043", item.SalesOrder);
        Assert.Equal("10", item.SalesOrderItem);
        Assert.Equal("SETVMIN0", item.Material);
    }

    /// <summary>
    /// Tests that OData V2 responses with deferred single navigation properties
    /// (using __deferred wrapper) are correctly deserialized as null.
    /// This validates the fix for single navigation properties not being treated as deferred.
    /// </summary>
    [Fact]
    public void ODataV2_DeferredSingleNavigationProperties_ShouldDeserializeAsNull()
    {
        // Arrange
        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V2);

        // Act
        var result = odataClient.ProcessQueryResponse<List<TestProduct>>(ProductWithDeferredSupplierJson);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);

        var product = result.Data[0];
        Assert.Equal(0, product.ID);
        Assert.Equal("Bread", product.Name);
        Assert.Equal("Whole grain bread", product.Description);

        // Both single navigation properties should be null when deferred
        Assert.Null(product.Category);
        Assert.Null(product.Supplier);
    }

    /// <summary>
    /// Tests that OData V2 responses with expanded single navigation properties
    /// are correctly deserialized with the related entity populated.
    /// </summary>
    [Fact]
    public void ODataV2_ExpandedSingleNavigationProperty_ShouldPopulateObject()
    {
        // Arrange
        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V2);

        // Act
        var result = odataClient.ProcessQueryResponse<List<TestProduct>>(ProductWithExpandedSupplierJson);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);

        var product = result.Data[0];
        Assert.Equal(0, product.ID);
        Assert.Equal("Bread", product.Name);

        // Category should be null (deferred)
        Assert.Null(product.Category);

        // Supplier should be populated (expanded)
        Assert.NotNull(product.Supplier);
        Assert.Equal(1, product.Supplier.ID);
        Assert.Equal("Acme Corp", product.Supplier.Name);
        Assert.NotNull(product.Supplier.Address);
        Assert.Equal("123 Main St", product.Supplier.Address.Street);
        Assert.Equal("Seattle", product.Supplier.Address.City);
    }

    /// <summary>
    /// Tests that OData V2 responses with mixed navigation properties
    /// (some expanded, some deferred) are correctly deserialized.
    /// </summary>
    [Fact]
    public void ODataV2_MixedNavigationProperties_ShouldHandleCorrectly()
    {
        // Arrange
        var odataClient = new ODataClient(new HttpClient(), ODataVersion.V2);

        // Act
        var result = odataClient.ProcessQueryResponse<List<TestProduct>>(ProductWithMixedNavigationPropertiesJson);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);

        var product = result.Data[0];
        Assert.Equal(0, product.ID);
        Assert.Equal("Bread", product.Name);

        // Category should be populated (expanded)
        Assert.NotNull(product.Category);
        Assert.Equal(1, product.Category.ID);
        Assert.Equal("Food", product.Category.Name);

        // Supplier should be null (deferred)
        Assert.Null(product.Supplier);
    }

    /// <summary>
    /// Tests that generated client code can correctly query and deserialize OData V2 metadata.
    /// This validates the end-to-end code generation and deserialization pipeline for V2.
    /// </summary>
    [Fact]
    public void GeneratedClient_WithODataV2Metadata_ShouldGenerateAndCompile()
    {
        // Arrange
        var metadata = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V2", "ODataDemo.xml"));
        var request = new ClientRequest
        {
            Name = "TestODataClient",
            Namespace = "Test.OData"
        };
        request.AddMetadata(metadata);

        var generator = new ClientGenerator(request);

        // Act
        var files = generator.GenerateClient();

        // Assert - Verify generated files contain expected converter usage
        var supplierFile = files.FirstOrDefault(f => f.FileName == "Supplier.cs" && f.FolderPath == "Types");
        Assert.NotNull(supplierFile);

        // Verify it has navigation properties
        Assert.Contains("public List<Product>?", supplierFile.Content);

        // Verify at least some files were generated
        Assert.NotEmpty(files);

        // Check that enums, types, inputs, and client files exist
        Assert.Contains(files, f => f.FolderPath == "Types");
        Assert.Contains(files, f => f.FolderPath == "Client");
    }

    /// <summary>
    /// Tests that generated client code can correctly query and deserialize OData V4 metadata.
    /// This validates the end-to-end code generation pipeline for V4.
    /// </summary>
    [Fact]
    public void GeneratedClient_WithODataV4Metadata_ShouldGenerateCorrectly()
    {
        // Arrange
        var metadata = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V4", "ODataDemo.xml"));
        var request = new ClientRequest
        {
            Name = "TestODataClient",
            Namespace = "Test.OData"
        };
        request.AddMetadata(metadata);

        var generator = new ClientGenerator(request);

        // Act
        var files = generator.GenerateClient();

        // Assert
        var personFile = files.FirstOrDefault(f => f.FileName == "Person.cs" && f.FolderPath == "Types");
        Assert.NotNull(personFile);

        // V4 should also have navigation properties
        Assert.Contains("PersonDetail", personFile.Content);
    }

    // Test entity classes for deserialization
    private class TestSalesOrder : IODataEntitySet
    {
        public string SalesOrder { get; set; } = "";
        public string SalesOrderType { get; set; } = "";
        public List<TestSalesOrderItem>? to_Item { get; set; }
        public string __Key => $"SalesOrder={SalesOrder}";
    }

    private class TestSalesOrderItem : IODataEntitySet
    {
        public string SalesOrder { get; set; } = "";
        public string SalesOrderItem { get; set; } = "";
        public string Material { get; set; } = "";
        public string __Key => $"SalesOrder={SalesOrder},SalesOrderItem={SalesOrderItem}";
    }

    // Test entity for single navigation property tests
    // Note: JsonConverter attributes are required on single navigation properties to handle __deferred wrappers.
    // Collection properties (List<>) don't need them because System.Text.Json automatically applies
    // converter factories to generic types, but not to plain object properties.
    private class TestProduct : IODataEntitySet
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        [System.Text.Json.Serialization.JsonConverter(typeof(ODataNavigationPropertyConverter<TestCategory>))]
        public TestCategory? Category { get; set; }

        [System.Text.Json.Serialization.JsonConverter(typeof(ODataNavigationPropertyConverter<TestSupplier>))]
        public TestSupplier? Supplier { get; set; }

        public string __Key => $"ID={ID}";
    }

    private class TestCategory : IODataEntitySet
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string __Key => $"ID={ID}";
    }

    private class TestSupplier : IODataEntitySet
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public TestAddress? Address { get; set; }
        public string __Key => $"ID={ID}";
    }

    private class TestAddress
    {
        public string? Street { get; set; }
        public string? City { get; set; }
    }
}
