using DemoClientV4.ODataDemo;
using Linq2OData.Generator.Models;
using Linq2OData.TestClients.AdHocClient;

namespace Linq2OData.TestClients
{
    internal class Program
    {

        const string demoUrlV2 = "https://services.odata.org/V2/(S(jo0zj0zu5nmnrfcfj2zv1ny2))/OData/OData.svc/";
        const string demoUrlV4 = "https://services.odata.org/V4/(S(jo0zj0zu5nmnrfcfj2zv1ny2))/OData/OData.svc/";
        const string tripPinUrl = "https://services.odata.org/V4/(S(0qbgsoxblavd1okt2uawjtfe))/TripPinServiceRW/";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Here we go!");

            // await GenerateDemoClientV2Async();
            // await GenerateDemoClientV4Async();
            // await GenerateTripPinClientAsync();

            await TestTripPinAsync();
            // await TestV4ClientAsync();
        }

        private static async Task TestTripPinAsync()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(tripPinUrl)
            };

            var tripPinClient = new TripPin.TripPinClient(httpClient);

            var result = await tripPinClient
                .Query<TripPin.Microsoft.OData.SampleService.Models.TripPin.Person>()
                .Top(1)
                .Expand(e => e.Trips!.Select(e => e.PlanItems))

                .ExecuteAsync();

            Console.WriteLine($"Success! Got {result?.Count} people with {result?[0].Trips?.Count} trips");
        }

        private static async Task TestV2ClientAsync()
        {

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(demoUrlV2)
            };

            var clientV2 = new DemoClientV2.ODataDemoClientV2(httpClient);

            var kalle = clientV2.Services;

            var queryResult = await clientV2
               .Query<DemoClientV2.ODataDemo.Product>()
               .Filter(e => e.ID != 1)
               .Expand(expand => expand.Category!.Products)
               .ExecuteAsync();


            var r = queryResult;


        }

        private static async Task TestV4ClientAsync()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(demoUrlV4)
            };

            var clientV4 = new DemoClientV4.ODataDemoClientV4(httpClient);

            var queryResult = await clientV4
            .Get<Product>(e => e.ID = 4)
            .Expand(e => e.Categories)
            .Expand(e => e.Supplier)
            .ExecuteAsync();

            var r = queryResult;

        }




        private static async Task GenerateDemoClientV2Async()
        {
            var httpClient = new HttpClient();
            var metadata = await httpClient.GetStringAsync(demoUrlV2 + "$metadata");

            var request = new Linq2OData.Generator.Models.ClientRequest
            {
                Name = "ODataDemoClientV2",
                Namespace = "DemoClientV2",
            };
            request.AddMetadata(metadata);

            var generator = new Linq2OData.Generator.ClientGenerator(request);

            var projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
            if (projectDirectory == null) { throw new Exception("Unable to get project directory"); }
            var files = generator.GenerateClient(Path.Combine(projectDirectory, "DemoClientV2"));
        }

        private static async Task GenerateDemoClientV4Async()
        {
            var httpClient = new HttpClient();
            var metadata = await httpClient.GetStringAsync(demoUrlV4 + "$metadata");

            var request = new Linq2OData.Generator.Models.ClientRequest
            {
                Name = "ODataDemoClientV4",
                Namespace = "DemoClientV4",
            };
            request.AddMetadata(metadata);

            var generator = new Linq2OData.Generator.ClientGenerator(request);


            var projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
            if (projectDirectory == null) { throw new Exception("Unable to get project directory"); }
            var files = generator.GenerateClient(Path.Combine(projectDirectory, "DemoClientV4"));
        }

        private static async Task GenerateTripPinClientAsync()
        {
            var httpClient = new HttpClient();
            var metadata = await httpClient.GetStringAsync(tripPinUrl + "$metadata");

            var request = new Linq2OData.Generator.Models.ClientRequest
            {
                Name = "TripPinClient",
                Namespace = "TripPin",
            };
            request.AddMetadata(metadata);

            var generator = new Linq2OData.Generator.ClientGenerator(request);


            var projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
            if (projectDirectory == null) { throw new Exception("Unable to get project directory"); }
            var files = generator.GenerateClient(Path.Combine(projectDirectory, "TripPinClientV4"));
        }

    }
}
