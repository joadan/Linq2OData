

namespace Linq2OData.DemoClient;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        await GenerateClientAsync();
   
        await TestClientAsync();

    }

    private static async Task TestClientAsync()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://services.odata.org/V2/OData/OData.svc/");

        var odataDemoClient = new GeneratedClient.ODataDemoClient(httpClient);

        var result = await odataDemoClient
             .ODataDemoEndpoint
             .GetSuppliers()
             .Top(5)
             .Select()
             .ExecuteBaseAsync();


        var cc = result?.Count() ?? 0;

    }


    private static async Task GenerateClientAsync()
    {
        var httpClient = new HttpClient();
        var metadata = await httpClient.GetStringAsync("https://services.odata.org/V2/OData/OData.svc/$metadata");

        var generator = new Linq2OData.Generator.ClientGenerator(
            new Linq2OData.Generator.Models.ClientRequest
            {
                Name = "ODataDemoClient",
                Namespace = "GeneratedClient",
                MetadataList = [metadata]
            });


        var projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
        if (projectDirectory == null) { throw new Exception("Unable to get project directory"); } 

        var files = generator.GenerateClient(projectDirectory + "/Generated");




    }
}
