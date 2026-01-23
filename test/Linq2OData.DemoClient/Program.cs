

namespace Linq2OData.DemoClient;

internal class Program
{
    //https://services.odata.org/V2/OData/OData.svc/
    const string demoUrl = "https://services.odata.org/V2/(S(ljasynfkiocpfy01gzrvjsra))/OData/OData.svc/";

    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

         //await GenerateClientAsync();

        await TestClientAsync();

    }

    private static async Task TestClientAsync()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(demoUrl)
        };

        var client = new GeneratedClient.ODataDemoClient(httpClient);

         await client.ODataDemo.ProductsDeleteAsync(1);

        var filteredResult = await client
            .ODataDemo
            .Products()
            .Top(3)
            .Filter("Rating eq 3")
            .Expand("Category, Supplier")
            .Select()
            .ExecuteAsync();


        var byKey = await client
           .ODataDemo
           .ProductsByKey(1)
           .Expand("Category")
           .Select()
           .ExecuteAsync();


        var rr = byKey;

    }


    private static async Task GenerateClientAsync()
    {
        var httpClient = new HttpClient();
        var metadata = await httpClient.GetStringAsync(demoUrl + "$metadata");

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
