using Linq2OData.Client.Converters;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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

        public async Task<T> CreateEntityAsync<T>(string entitysetName, ODataInputBase input)
        {
            string json = JsonSerializer.Serialize(input.GetValues(), jsonOptions);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"{entitysetName}", content);
         
            if (!response.IsSuccessStatusCode)
            {
                var contentResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error creating entity: {response.StatusCode}, Content: {contentResponse}");
            }

            //TODO: process response.. we should return the created entity

            return default!;


        }

        public async Task<bool> UpdateEntityAsync(string entitysetName, string keyExpression, ODataInputBase input)
        {

            var tt = input.GetValues();

            return false;

        }


    }
}
