using System.Text;
using System.Text.Json;

namespace Linq2OData.Client
{
    public static class ODataJsonCleanupHelper
    {
        /// <summary>
        /// Removes OData system properties (e.g. __metadata, __deferred)
        /// and completely drops objects that only contain metadata.
        /// </summary>
        public static string Clean(string json)
        {
            using var document = JsonDocument.Parse(json);
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);

            WriteCleanElement(writer, document.RootElement);

            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private static void WriteCleanElement(
            Utf8JsonWriter writer,
            JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    writer.WriteStartObject();

                    foreach (var prop in element.EnumerateObject())
                    {
                        // Remove OData system properties
                        if (prop.Name.StartsWith("__") && prop.Name != "__count")
                            continue;

                        // Drop metadata-only objects completely
                        if (IsMetadataOnlyObject(prop.Value))
                            continue;

                        writer.WritePropertyName(prop.Name);
                        WriteCleanElement(writer, prop.Value);
                    }

                    writer.WriteEndObject();
                    break;

                case JsonValueKind.Array:
                    writer.WriteStartArray();

                    foreach (var item in element.EnumerateArray())
                        WriteCleanElement(writer, item);

                    writer.WriteEndArray();
                    break;

                default:
                    WritePrimitive(writer, element);
                    break;
            }
        }

        private static bool IsMetadataOnlyObject(JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.Object)
                return false;

            foreach (var prop in element.EnumerateObject())
            {
                if (!prop.Name.StartsWith("__"))
                    return false;
            }

            return true;
        }

        private static void WritePrimitive(
            Utf8JsonWriter writer,
            JsonElement element)
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
