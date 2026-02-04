using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linq2OData.Core.Converters
{
    /// <summary>
    /// JSON converter for OData V2/V3 single navigation properties that handles the "__deferred" wrapper.
    /// In OData V2/V3, non-expanded navigation properties have a "__deferred" object with a URI.
    /// Expanded properties are just the object itself.
    /// </summary>
    public class ODataNavigationPropertyConverter<T> : JsonConverter<T> where T : class
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            // If it's an object, check if it's a __deferred reference
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // We need to peek at the JSON to see if it has __deferred
                // Use JsonDocument which doesn't advance the reader position incorrectly
                var readerCopy = reader;
                using (JsonDocument doc = JsonDocument.ParseValue(ref readerCopy))
                {
                    // Check if this is a __deferred reference (non-expanded)
                    if (doc.RootElement.TryGetProperty("__deferred", out _))
                    {
                        // Navigation property was not expanded, skip the JSON and return null
                        // We need to advance the original reader past this object
                        reader.Skip();
                        return null;
                    }

                    // It's an expanded navigation property
                    // We need to deserialize without this converter to avoid recursion
                    // Read the JSON manually without using the converter
                    var json = doc.RootElement.GetRawText();

                    // Create new options without the navigation property converter
                    var newOptions = new JsonSerializerOptions(options);
                    for (int i = newOptions.Converters.Count - 1; i >= 0; i--)
                    {
                        if (newOptions.Converters[i] is ODataNavigationPropertyConverterFactory)
                        {
                            newOptions.Converters.RemoveAt(i);
                        }
                    }

                    // Advance the reader to consume the object
                    reader.Skip();

                    return JsonSerializer.Deserialize<T>(json, newOptions);
                }
            }

            throw new JsonException($"Unexpected JSON token when deserializing OData navigation property: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                JsonSerializer.Serialize(writer, value, options);
            }
        }
    }
}
