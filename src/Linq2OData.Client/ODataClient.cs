using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2OData.Client
{
    public class ODataClient
    {

        private HttpClient httpClient;
        public ODataClient(HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            this.httpClient = httpClient;
        }


        public HttpClient HttpClient => httpClient;

    }
}
