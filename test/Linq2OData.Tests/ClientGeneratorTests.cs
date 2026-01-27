using Linq2OData.Generator;
using Linq2OData.Generator.Models;

namespace Linq2OData.Tests;

public class ClientGeneratorTests
{

    private string odataDemoMetadataV2;
    private string odataDemoMetadataV4;
    private string sapSalesQuotationMetadataV2;
    private string largeMetadaV4;

    public ClientGeneratorTests()
    {

        odataDemoMetadataV2 = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V2", "ODataDemo.xml"));
        odataDemoMetadataV4 = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V4", "ODataDemo.xml"));
        sapSalesQuotationMetadataV2 = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V2", "SapSalesQuotation.xml"));
        largeMetadaV4 = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V4", "LargeMetadata.xml"));
    }

    [Fact]
    public void GenerateClientAsyncV2_WithValidMetadata_ShouldGenerateFiles()
    {
   
        var request = new ClientRequest
        {
            Name = "ODataDemoClient",
            Namespace = "MyApp.OData",
            MetadataList = [odataDemoMetadataV2]
        };

        var generator = new ClientGenerator(request);

        // Act
        var files = generator.GenerateClient();

        var supplier = files.First(f => f.FileName == "Supplier.cs" && f.FolderPath == "Types"); 

        Console.WriteLine($"{supplier.Content}");

        // Assert
        Assert.NotNull(files);
        Assert.NotEmpty(files);
    }

    [Fact]
    public void GenerateClientAsyncV4_WithValidMetadata_ShouldGenerateFiles()
    {

        var request = new ClientRequest
        {
            Name = "ODataDemoClient",
            Namespace = "MyApp.OData",
            MetadataList = [odataDemoMetadataV4]
        };

        var generator = new ClientGenerator(request);

        // Act
        var files = generator.GenerateClient();

        var supplier = files.First(f => f.FileName == "Supplier.cs" && f.FolderPath == "Types");

        Console.WriteLine($"{supplier.Content}");

        // Assert
        Assert.NotNull(files);
        Assert.NotEmpty(files);
    }

    [Fact]
    public void GenerateClientAsyncV4_WithValidLargeMetadata_ShouldGenerateFiles()
    {

        var request = new ClientRequest
        {
            Name = "ODataLargeClient",
            Namespace = "MyApp.OData",
            MetadataList = [largeMetadaV4]
        };

        var generator = new ClientGenerator(request);

        // Act
        var files = generator.GenerateClient();

        
        // Assert
        Assert.NotNull(files);
        Assert.NotEmpty(files);
    }

}
