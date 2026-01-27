

namespace DemoClientV4;

public class ODataDemoClientV4 
{
    private Linq2OData.Client.ODataClient odataClient;
    public Linq2OData.Client.ODataClient ODataClient => odataClient;

    public ODataDemoClientV4(HttpClient httpClient) 
    {
         odataClient = new Linq2OData.Client.ODataClient(httpClient, Linq2OData.Client.ODataVersion.V4); 
              ODataDemo = new ODataDemo.ODataDemoEndpoint(odataClient);
       
    }

        public ODataDemo.ODataDemoEndpoint ODataDemo { get; set; }
}