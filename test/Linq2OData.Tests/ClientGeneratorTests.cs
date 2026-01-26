using Linq2OData.Generator;
using Linq2OData.Generator.Models;

namespace Linq2OData.Tests;

public class ClientGeneratorTests
{

    private string odataDemoMetadataV2;
    private string odataDemoMetadataV4;
    private string sapSalesQuotationMetadataV2;

    public ClientGeneratorTests()
    {

        odataDemoMetadataV2 = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V2", "ODataDemo.xml"));
        odataDemoMetadataV4 = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V4", "ODataDemo.xml"));
        sapSalesQuotationMetadataV2 = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V2", "SapSalesQuotation.xml"));
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

}
