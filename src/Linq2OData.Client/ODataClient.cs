using Linq2OData.Client.Converters;
using System;
using System.Collections.Generic;
using System.Net;
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
            jsonOptions.Converters.Add(new MicrosoftNullableDateTimeConverter());
            jsonOptions.Converters.Add(new MicrosoftDateTimeOffsetConverter());

            jsonOptions.Converters.Add(new DecimalStringJsonConverter());
          //  jsonOptions.Converters.Add(new Int32StringJsonConverter());
            //jsonOptions.Converters.Add(new NullableInt32StringJsonConverter());
            jsonOptions.Converters.Add(new Int64StringJsonConverter());
            jsonOptions.Converters.Add(new NullableInt64StringJsonConverter());

        }
        public HttpClient HttpClient => httpClient;
        public JsonSerializerOptions JsonOptions => jsonOptions;


        public async Task<bool> DeleteEntityAsync(string entitysetName, string keyExpression)
        {
            var response = await httpClient.DeleteAsync($"{entitysetName}({keyExpression})");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) { return false; }
            await ValidateResponseAsync(response);
            return true;
        }

        public async Task<T> CreateEntityAsync<T>(string entitysetName, ODataInputBase input)
        {
            string json = JsonSerializer.Serialize(input.GetValues(), jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync($"{entitysetName}", content);

            await ValidateResponseAsync(response);

            var rawResponse = await response.Content.ReadAsStringAsync();
            var odataResponse = ProcessQueryResponse<T>(rawResponse);

            if (odataResponse == null || odataResponse.Data == null)
            {
                throw new Exception("Create entity reported success but the response is null.");
            }


            return odataResponse!.Data!;


        }

        public async Task<bool> UpdateEntityAsync(string entitysetName, string keyExpression, ODataInputBase input)
        {
            string json = JsonSerializer.Serialize(input.GetValues(), jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            

            var request = new HttpRequestMessage(new HttpMethod("MERGE"), $"{entitysetName}({keyExpression})")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) { return false; }

            await ValidateResponseAsync(response);

            return true;

        }


        public async Task<ODataResponse<T>?> QueryEntityAsync<T>(string url, CancellationToken token = default)
        {

            using var response = await httpClient.GetAsync(url, token);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                await ValidateResponseAsync(response);
            }

            var rawResponse = await response.Content.ReadAsStringAsync(token);
            return ProcessQueryResponse<T>(rawResponse);
        }

        private ODataResponse<T>? ProcessQueryResponse<T>(string rawResponse)
        {
            var cleanedJson = ODataJsonCleanupHelper.Clean(rawResponse);
            return JsonSerializer.Deserialize<ODataResponse<T>>(cleanedJson, jsonOptions)!;
        }


        private async Task ValidateResponseAsync(HttpResponseMessage response)
        {

            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var content = await response.Content.ReadAsStringAsync();


            ODataErrorResponse? odataError = null;
            try
            {
                odataError = JsonSerializer.Deserialize<ODataErrorResponse>(content);
            }
            catch
            {
            }
           
            var ex = new ODataRequestException($"OData request failed status code: {(int)response.StatusCode} Error: {odataError?.Error?.Message?.Value} InnerError: {odataError?.Error?.InnerError?.Message}", response.RequestMessage,  odataError);

            throw ex;
        }
    }



}
