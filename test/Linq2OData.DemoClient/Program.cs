

using GeneratedClient.ODataDemo;
using System.Runtime.CompilerServices;

namespace Linq2OData.DemoClient;

internal class Program
{
    const string demoUrl = "https://services.odata.org/V2/(S(jo0zj0zu5nmnrfcfj2zv1ny2))/OData/OData.svc/";

    static async Task Main(string[] args)
    {
        Console.WriteLine("Here we go..");
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
    
        //var error = await client
        // .ODataDemo
        // .Products()
        // .Top(30)
        // .Expand("Error")
        // .Select()
        // .ExecuteAsync();


        ////Query entities
        //var filteredResult = await client
        //    .ODataDemo
        //    .Products()
        //    .Top(30)
        //    .Filter(e => e.Rating >= 3 || e.ID == 999)
        //    .Expand("Category")
        //    .Select(e => e.Select(f => new { f.Rating, f.ID }))
        //    .ExecuteAsync();

        //var rr = filteredResult;

        ////Update an entity
        //var result = await client
        //    .ODataDemo
        //    .ProductsUpdateAsync(1, new ProductInput
        //    {
        //        Name = "Test Product__",
        //    });

        ////Select an entity by key
        //var product = await client.ODataDemo
        //        .ProductsByKey(1)
        //        .Expand("Category")
        //        .Select()

        //        .ExecuteAsync();


        //Create a new entity
        var newProduct = await client.ODataDemo.ProductsCreateAsync(new ProductInput
        {
            ID = 1000,
            Name = "Test Product",
            Description = "This is a my test product",
            Rating = 5,
            ReleaseDate = DateTime.Now,
            Price = 10,
        });


        var mm = newProduct;

        ////Delete an entity by key
        //await client.ODataDemo.ProductsDeleteAsync(1);








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
