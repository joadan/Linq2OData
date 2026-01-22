using Linq2OData.Generator;
using Linq2OData.Generator.Models;

namespace Linq2OData.Tests;

public class ClientGeneratorTests
{

    private string odataDemoMetadataV2;
    private string sapSalesQuotationMetadataV2;

    public ClientGeneratorTests()
    {

        odataDemoMetadataV2 = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V2", "ODataDemo.xml"));
        sapSalesQuotationMetadataV2 = File.ReadAllText(Path.Combine("SampleData", "Metadata", "V2", "SapSalesQuotation.xml"));
    }

    [Fact]
    public void GenerateClientAsync_WithValidMetadata_ShouldGenerateFiles()
    {
        // Arrange
        

        var request = new ClientRequest
        {
            Name = "ODataDemoClient",
            Namespace = "MyApp.OData",
            MetadataList = [odataDemoMetadataV2]
        };

        var generator = new ClientGenerator(request);

        // Act
        var files = generator.GenerateClient();

        var supplier = files.First(f => f.FileName == "Supplier" && f.FolderPath == "Types"); 

        Console.WriteLine($"{supplier.Content}");

        // Assert
        Assert.NotNull(files);
        Assert.NotEmpty(files);
    }

}
