using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;

namespace Linq2OData.Client
{
    internal class QueryExecutor()
    {
        private const string DataPropertyName = "d";
        private const string ResultsPropertyName = "results";
        internal async Task<T?> ExecuteRequestAsync<T>(ODataQuery<T> oDataQuery, CancellationToken token = default)
        {
            using var response = await oDataQuery.ODataClient.HttpClient.GetAsync(oDataQuery.GenerateRequestUrl(), token);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(token);
                //throw new GraphQueryRequestException($"Http error! Status code {response.StatusCode} Error: {content}",
                //    graphRequest.Query, graphRequest.Variables);
            }

            var rawResponse = await response.Content.ReadAsStringAsync(token);
         
            return ProcessResponse<T>(rawResponse);
        }



        public T? ProcessResponse<T>(string con)
        {
            //var document = JsonDocument.Parse(con);

            var root = JsonDocument.Parse(con).RootElement;

            root.TryGetProperty(DataPropertyName, out var dataElement);


            if (dataElement.ValueKind == JsonValueKind.Array)
            {
                return DeserializeArray<T>(root);
            }

            throw new InvalidOperationException("Unexpected OData payload");
        }


        private T DeserializeArray<T>(JsonElement array)
        {
            var cleanedJson = RemoveODataMetadata(array);
            return JsonSerializer.Deserialize<T>(cleanedJson)!;
        }


        private static string RemoveODataMetadata(JsonElement element)
        {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            WriteWithoutMetadata(writer, element);

            writer.Flush();
            return System.Text.Encoding.UTF8.GetString(stream.ToArray());
        }

        private static void WriteWithoutMetadata(Utf8JsonWriter writer, JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    writer.WriteStartObject();
                    foreach (var prop in element.EnumerateObject())
                    {
                        if (prop.Name.StartsWith("__")) continue;

                        writer.WritePropertyName(prop.Name);
                        WriteWithoutMetadata(writer, prop.Value);
                    }
                    writer.WriteEndObject();
                    break;

                case JsonValueKind.Array:
                    writer.WriteStartArray();
                    foreach (var item in element.EnumerateArray())
                    {
                        WriteWithoutMetadata(writer, item);
                    }
                    writer.WriteEndArray();
                    break;

                default:
                    propWriteValue(writer, element);
                    break;
            }
        }

        private static void propWriteValue(Utf8JsonWriter writer, JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    writer.WriteStringValue(element.GetString());
                    break;
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out var l))
                        writer.WriteNumberValue(l);
                    else
                        writer.WriteNumberValue(element.GetDouble());
                    break;
                case JsonValueKind.True:
                case JsonValueKind.False:
                    writer.WriteBooleanValue(element.GetBoolean());
                    break;
                case JsonValueKind.Null:
                    writer.WriteNullValue();
                    break;
            }
        }


    }
}
