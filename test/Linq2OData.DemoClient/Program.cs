

//using DemoClientV2;
//using DemoClientV4;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Linq2OData.DemoClient;

internal class Program
{
    const string demoUrlV2 = "https://services.odata.org/V2/(S(jo0zj0zu5nmnrfcfj2zv1ny2))/OData/OData.svc/";
    const string demoUrlV4 = "https://services.odata.org/V4/(S(jo0zj0zu5nmnrfcfj2zv1ny2))/OData/OData.svc/";
    static async Task Main(string[] args)
    {
        Console.WriteLine("Here we go..");
        //await GenerateDemoClientV2Async();
        await GenerateDemoClientV4Async();
        //  await TestV4ClientAsync();

    }

    //private static async Task TestV4ClientAsync()
    //{
    //    var httpClient = new HttpClient
    //    {
    //        BaseAddress = new Uri(demoUrlV4)
    //    };

       

    //    var clientV4 = new ODataDemoClientV4(httpClient);


    //    //Test raw client
    //    var rawResult = await clientV4.ODataClient.QueryEntitySetAsync<JsonElement>("Products");
    //    var rawEntity = await clientV4.ODataClient.QueryEntityAsync<JsonElement>("Products", "ID=99999");


    //    var products = await clientV4
    //     .ODataDemo
    //     .Products()
    //     .Select()
    //     .ExecuteAsync();



    //    //Query entities
    //    var filteredResult = await clientV4
    //        .ODataDemo
    //        .Products()
    //        .Top(30)
    //        .Filter(e => e.Rating >= 3 || e.ID == 999)
    //        .Expand("Categories")
    //        .Select(e => e.Select(f => new { f.Rating, f.ID }))
    //        .ExecuteAsync();

    //    var rr = filteredResult;

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


        ////Create a new entity
        //var newProduct = await client.ODataDemo.ProductsCreateAsync(new ProductInput
        //{
        //    ID = 1000,
        //    Name = "Test Product",
        //    Description = "This is a my test product",
        //    Rating = 5,
        //    Price = 10,
        //   ReleaseDate = DateTime.Now,
        //});


        //var mm = newProduct;

        ////Delete an entity by key
        //await client.ODataDemo.ProductsDeleteAsync(1000);








    //}


    private static async Task GenerateDemoClientV2Async()
    {
        var httpClient = new HttpClient();
        var metadata = await httpClient.GetStringAsync(demoUrlV2 + "$metadata");

        var generator = new Linq2OData.Generator.ClientGenerator(
            new Linq2OData.Generator.Models.ClientRequest
            {
                Name = "ODataDemoClientV2",
                Namespace = "DemoClientV2",
                MetadataList = [metadata]
            });


        var projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
        if (projectDirectory == null) { throw new Exception("Unable to get project directory"); }

        var files = generator.GenerateClient(projectDirectory + "/DemoClientV2");
    }

    private static async Task GenerateDemoClientV4Async()
    {
        var httpClient = new HttpClient();
        var metadata = await httpClient.GetStringAsync(demoUrlV4 + "$metadata");

        var generator = new Linq2OData.Generator.ClientGenerator(
            new Linq2OData.Generator.Models.ClientRequest
            {
                Name = "ODataDemoClientV4",
                Namespace = "DemoClientV4",
                MetadataList = [metadata]
            });


        var projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
        if (projectDirectory == null) { throw new Exception("Unable to get project directory"); }

        var files = generator.GenerateClient(projectDirectory + "/DemoClientV4");
    }

}
