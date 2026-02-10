using Linq2OData.Core.Converters;
using Linq2OData.Core.ODataResponse;
using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Linq2OData.Core
{
    public class ODataClient
    {

        private HttpClient httpClient;
        private ODataVersion odataVersion;
        private JsonSerializerOptions jsonOptions;

        public ODataClient(HttpClient httpClient, ODataVersion odataVersion)
        {
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            this.httpClient = httpClient;
            this.odataVersion = odataVersion;
            jsonOptions = new JsonSerializerOptions();

            // Add ODataInputBase converter factory for all versions to handle nested Input objects
            jsonOptions.Converters.Add(new ODataInputBaseConverterFactory());

            if (odataVersion < ODataVersion.V4) //Not really sure about this but I belive it is a good start
            {
                jsonOptions.Converters.Add(new MicrosoftDateTimeConverter());
                jsonOptions.Converters.Add(new MicrosoftNullableDateTimeConverter());
                jsonOptions.Converters.Add(new MicrosoftDateTimeOffsetConverter());
                jsonOptions.Converters.Add(new DecimalStringJsonConverter());
                jsonOptions.Converters.Add(new Int64StringJsonConverter());
                jsonOptions.Converters.Add(new NullableInt64StringJsonConverter());
                // Add the collection converter for handling "results" wrapper in navigation properties (read/write)
                jsonOptions.Converters.Add(new ODataCollectionConverterFactory());
                // Add the navigation property converter for handling "__deferred" wrapper in non-expanded properties
                jsonOptions.Converters.Add(new ODataNavigationPropertyConverterFactory());
            }


        }
        public HttpClient HttpClient => httpClient;
        public ODataVersion ODataVersion => odataVersion;
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
            string json = JsonSerializer.Serialize(input, jsonOptions);
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
            string json = JsonSerializer.Serialize(input, jsonOptions);
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


        public async Task<ODataResponse<List<T>>?> QueryEntitySetAsync<T>(string entitySetName, string? select, string? expand = null, string? filter = null, bool? count = null, int? top = null, int? skip = null, string? orderby = null, CancellationToken token = default)
        {
            var url = GenerateUrl(entitySetName: entitySetName, select: select, expand: expand, filter: filter, count: count, top: top, skip: skip, orderby: orderby);

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
            return ProcessQueryResponse<List<T>>(rawResponse);
        }


        public async Task<ODataResponse<T>?> QueryEntityAsync<T>(string entitySetName, string keyString, string? select = null, string? expand = null, CancellationToken token = default)
        {
            var url = GenerateUrl(entitySetName: entitySetName, keyString: keyString, select: select, expand: expand);

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

        private string GenerateUrl(string entitySetName, string? keyString = null, string? select = null, string? expand = null, string? filter = null, bool? count = null, int? top = null, int? skip = null, string? orderby = null)
        {

            var urlBuilder = new StringBuilder();
            urlBuilder.Append(entitySetName);

            if (!string.IsNullOrWhiteSpace(keyString))
            {
                urlBuilder.Append($"({keyString})");
            }

            var queryParameters = new List<string>();
            if (top > 0)
            {
                queryParameters.Add($"$top={top}");
            }

            if (skip > 0)
            {
                queryParameters.Add($"$skip={skip}");
            }

            if (!string.IsNullOrWhiteSpace(select))
            {
                queryParameters.Add($"$select={select}");
            }

            if (!string.IsNullOrWhiteSpace(expand))
            {
                queryParameters.Add($"$expand={expand}");
            }

            if (!string.IsNullOrWhiteSpace(filter))
            {
                queryParameters.Add($"$filter={filter}");
            }

            if (!string.IsNullOrWhiteSpace(orderby))
            {
                queryParameters.Add($"$orderby={orderby}");
            }

            if (count == true)
            {
                if (odataVersion == ODataVersion.V4)
                {
                    queryParameters.Add($"$count=true");
                }
                else
                {
                    queryParameters.Add($"inlinecount=allpages");
                }
            }

            if (queryParameters.Count > 0)
            {
                urlBuilder.Append("?");
                urlBuilder.Append(string.Join("&", queryParameters));
            }
            return urlBuilder.ToString();
        }




        internal ODataResponse<T>? ProcessQueryResponse<T>(string rawResponse)
        {
            if (string.IsNullOrWhiteSpace(rawResponse))
            {
                return null;
            }

            //if (odataVersion != ODataVersion.V4)
            //{
            //    rawResponse = ODataJsonCleanupHelper.Clean(rawResponse);
            //}

            JsonNode? root = JsonNode.Parse(rawResponse);

            if (root == null)
            {
                return null;
            }

            var isCollection = IsCollection<T>();

            if (odataVersion == ODataVersion.V4)
            {
                return ProcessQueryResponseV4<T>(root, isCollection);
            }
            else
            {
                return ProcessQueryResponseV1_3<T>(root, isCollection);
            }
        }

        bool IsCollection<T>()
        {
            return typeof(IEnumerable).IsAssignableFrom(typeof(T));
        }

        private ODataResponse<T>? ProcessQueryResponseV4<T>(JsonNode root, bool isCollection)
        {
            var response = new ODataResponse<T>();
            if (isCollection)
            {

                var results = root["value"];
                if (results == null)
                {
                    return null;
                }
                response.Data = results.Deserialize<T>(jsonOptions);
                var countNode = root["@odata.count"];
                if (countNode != null)
                {
                    if (long.TryParse(countNode.AsValue().ToString(), out var countValue))
                    {
                        response.Count = countValue;
                    }
                }
            }
            else
            {
                root = ReorderODataTypeFirst(root);
                response.Data = root.Deserialize<T>(jsonOptions);
            }

            return response;

        }

        private JsonNode ReorderODataTypeFirst(JsonNode node)
        {
            if (node is not JsonObject jsonObject) return node;

            var odataType = jsonObject["@odata.type"];
            if (odataType == null) return node;

            var reordered = new JsonObject();
            reordered["@odata.type"] = odataType.DeepClone();

            foreach (var property in jsonObject)
            {
                if (property.Key != "@odata.type")
                {
                    reordered[property.Key] = property.Value?.DeepClone();
                }
            }

            return reordered;
        }

        private ODataResponse<T>? ProcessQueryResponseV1_3<T>(JsonNode root, bool isCollection)
        {
            var d = root["d"];
            if (d == null) { return null; }

            JsonNode? results = null;
            if (d.GetValueKind() == JsonValueKind.Object)
            {
                results = d["results"];
            }

            var response = new ODataResponse<T>();
            if (results == null)
            {
                response.Data = d.Deserialize<T>(jsonOptions)!;
            }
            else
            {
                response.Data = results.Deserialize<T>(jsonOptions);

                var countNode = d["__count"];
                if (countNode != null)
                {
                    if (long.TryParse(countNode.AsValue().ToString(), out var countValue))
                    {
                        response.Count = countValue;
                    }
                }
            }
            return response;
        }


        private async Task ValidateResponseAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var content = await response.Content.ReadAsStringAsync();

            ODataError odataError = new ODataError();

            try
            {
                if (odataVersion == ODataVersion.V4)
                {
                    var odataErrorV4 = JsonSerializer.Deserialize<ODataErrorResponseV4>(content);
                    odataError.Message = odataErrorV4?.Error?.Message;
                    odataError.Code = odataErrorV4?.Error?.Code;
                    odataError.InnerError = new ODataInnerError
                    {
                        Message = odataErrorV4?.Error?.InnerError?.Message,
                        Type = odataErrorV4?.Error?.InnerError?.Type,
                        Stacktrace = odataErrorV4?.Error?.InnerError?.Stacktrace
                    };
                }
                else
                {
                    var odataErrorV1_3 = JsonSerializer.Deserialize<ODataErrorResponseV1_3>(content);
                    odataError.Message = odataErrorV1_3?.Error?.Message?.Value;
                    odataError.Code = odataErrorV1_3?.Error?.Message?.Lang; //Not sure this is a good idea..
                    odataError.InnerError = new ODataInnerError
                    {
                        Message = odataErrorV1_3?.Error?.InnerError?.Message,
                        Type = odataErrorV1_3?.Error?.InnerError?.Type,
                        Stacktrace = odataErrorV1_3?.Error?.InnerError?.Stacktrace
                    };
                }
            }
            catch (Exception ex)
            {
                odataError.Message = $"Failed to parse error response: {ex.Message}";
            }

            throw new ODataRequestException($"OData request failed. Status code:{(int)response.StatusCode}. Error:{odataError?.Message}", response.RequestMessage, odataError);
        }
    }

}
