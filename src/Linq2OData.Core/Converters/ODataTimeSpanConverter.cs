using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace Linq2OData.Core.Converters
{
    public class ODataTimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value))
            {
                return TimeSpan.Zero;
            }

            // Try ISO 8601 Duration format first (e.g., PT1H30M)
            if (value.StartsWith("P", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    return XmlConvert.ToTimeSpan(value);
                }
                catch
                {
                    // Fall through to standard TimeSpan parsing
                }
            }

            // Try standard TimeSpan format (e.g., "1:30:00")
            if (TimeSpan.TryParse(value, out var timeSpan))
            {
                return timeSpan;
            }

            // If all else fails, throw with a helpful message
            throw new JsonException($"Unable to parse TimeSpan value: {value}");
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            // Always write as ISO 8601 Duration format for OData compatibility
            writer.WriteStringValue(XmlConvert.ToString(value));
        }
    }

    public class ODataNullableTimeSpanConverter : JsonConverter<TimeSpan?>
    {
        public override TimeSpan? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            var value = reader.GetString();
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            // Try ISO 8601 Duration format first (e.g., PT1H30M)
            if (value.StartsWith("P", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    return XmlConvert.ToTimeSpan(value);
                }
                catch
                {
                    // Fall through to standard TimeSpan parsing
                }
            }

            // Try standard TimeSpan format (e.g., "1:30:00")
            if (TimeSpan.TryParse(value, out var timeSpan))
            {
                return timeSpan;
            }

            // If all else fails, throw with a helpful message
            throw new JsonException($"Unable to parse TimeSpan value: {value}");
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                // Always write as ISO 8601 Duration format for OData compatibility
                writer.WriteStringValue(XmlConvert.ToString(value.Value));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
