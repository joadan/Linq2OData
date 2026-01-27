

namespace DemoClientV2;

public class ODataDemoClientV2 
{
    private Linq2OData.Client.ODataClient odataClient;
    public Linq2OData.Client.ODataClient ODataClient => odataClient;

    public ODataDemoClientV2(HttpClient httpClient) 
    {
         odataClient = new Linq2OData.Client.ODataClient(httpClient, Linq2OData.Client.ODataVersion.V2); 
              ODataDemo = new ODataDemo.ODataDemoEndpoint(odataClient);
       
    }

        public ODataDemo.ODataDemoEndpoint ODataDemo { get; set; }
}