

using GeneratedClient.ODataDemo;

namespace Linq2OData.DemoClient;

internal class Program
{
    const string demoUrl = "https://services.odata.org/V2/(S(jo0zj0zu5nmnrfcfj2zv1ny2))/OData/OData.svc/";

    static async Task Main(string[] args)
    {
        Console.WriteLine("Here we go..");

         await GenerateClientAsync();

      //  await TestClientAsync();

    }

    private static async Task TestClientAsync()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(demoUrl)
        };

        var client = new GeneratedClient.ODataDemoClient(httpClient);

        //Query entities
        var filteredResult = await client
            .ODataDemo
            .Products()
            .Top(3)
            .Filter(e => e.Rating >= 3)
          //  .Expand("Category, Supplier")
            .Select(e => e.Select(f => new {f.Rating, f.ID}))
            .ExecuteAsync();

        var rr = filteredResult;

        ////Update an entity
        //var result = await client
        //    .ODataDemo
        //    .ProductsUpdateAsync(1, new ProductInput
        //    {
        //        Name = "Test Product1",
        //    });

        ////Select an entity by key
        //var product = await client.ODataDemo
        //        .ProductsByKey(1)
        //        .Select()
        //        .ExecuteAsync();


        ////Create a new entity
        //var newProduct = await client.ODataDemo.ProductsCreateAsync(new ProductInput
        //{
        //    ID = 999,
        //    Name = "Test Product",
        //    Description = "This is a test product",
        //    Rating = 5,
        //    Price = 10,
        //});



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
