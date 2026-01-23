using Linq2OData.Client.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Linq2OData.Client
{
    public class ODataClient
    {

        private HttpClient httpClient;

        private JsonSerializerOptions jsonOptions;

        public ODataClient(HttpClient httpClient)
        {

            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            this.httpClient = httpClient;

            jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new MicrosoftDateTimeConverter());
            jsonOptions.Converters.Add(new MicrosoftDateTimeOffsetConverter());

            jsonOptions.Converters.Add(new DecimalStringJsonConverter());
            jsonOptions.Converters.Add(new Int32StringJsonConverter());
            jsonOptions.Converters.Add(new Int64StringJsonConverter());

        }


        public HttpClient HttpClient => httpClient;
        public JsonSerializerOptions JsonOptions => jsonOptions;



        public async Task<bool> DeleteEntityAsync(string entitysetName, string keyExpression)
        {
            var result = await httpClient.DeleteAsync($"{entitysetName}({keyExpression})");

            if (result.StatusCode == System.Net.HttpStatusCode.NotFound) { return false; }

            result.EnsureSuccessStatusCode();
            return true;
        }

        public async Task CreateEntityAsync(string entitysetName, ODataInputBase input)
        {

            var tt = input.GetValues();


        }


    }
}
