using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linq2OData.Core.Converters
{
    /// <summary>
    /// JSON converter for OData V2/V3 collection navigation properties that handles the "results" wrapper.
    /// In OData V2/V3, expanded collections are wrapped in a "results" object like: { "results": [...] }
    /// </summary>
    public class ODataCollectionConverter<T> : JsonConverter<List<T>>
    {
        public override List<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            // If it's already an array, deserialize directly (V4 format)
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var list = new List<T>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                    {
                        return list;
                    }

                    var item = JsonSerializer.Deserialize<T>(ref reader, options);
                    if (item != null)
                    {
                        list.Add(item);
                    }
                }
                throw new JsonException("Unexpected end of JSON array");
            }

            // If it's an object, look for "results" property (OData V2/V3)
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
                {
                    if (doc.RootElement.TryGetProperty("results", out JsonElement results))
                    {
                        var list = new List<T>();
                        foreach (var element in results.EnumerateArray())
                        {
                            var item = element.Deserialize<T>(options);
                            if (item != null)
                            {
                                list.Add(item);
                            }
                        }
                        return list;
                    }
                }
            }

            throw new JsonException($"Unexpected JSON token when deserializing OData collection: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            // For serialization, just write the array directly
            writer.WriteStartArray();
            foreach (var item in value)
            {
                JsonSerializer.Serialize(writer, item, options);
            }
            writer.WriteEndArray();
        }
    }
}
