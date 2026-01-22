
namespace GeneratedClient;

public class ODataDemoClient 
{
    public ODataDemoClient(HttpClient httpClient) 
    {
         var odataClient = new Linq2OData.Client.ODataClient(httpClient); 
              ODataDemoEndpoint = new ODataDemo.ODataDemoEndpoint(odataClient);
       
    }

        public ODataDemo.ODataDemoEndpoint ODataDemoEndpoint { get; set; }
}