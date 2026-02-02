



//using DemoClientV4.ODataDemo;
using DemoClientV4.ODataDemo;
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
        //await GenerateDemoClientV4Async();
        await TestV4ClientAsync();

    }

    private static async Task TestV4ClientAsync()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(demoUrlV4)
        };

        var clientV4 = new DemoClientV4.ODataDemoClientV4(httpClient);


        var result2 = await clientV4
           .Query<Person>()
           .Expand("PersonDetail")
           .Filter(e => e.ID > 4)
           .ExecuteAsync();



        //var t = await clientV4
        //    .Update<Person>(e => e.ID = 4)
        //    .ExecuteAsync(new PersonInput { Name = "Test" });


        var result = await clientV4
            .Get<Person>(e => e.ID = 4)
           .ExecuteAsync();

        var h = result;

    }


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
