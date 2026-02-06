using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linq2OData.Core.Converters
{
    /// <summary>
    /// JSON converter for ODataInputBase that ensures nested ODataInputBase objects
    /// are serialized by calling GetValues() recursively.
    /// This is important when Input classes contain other Input classes as navigation properties.
    /// </summary>
    public class ODataInputBaseConverter : JsonConverter<ODataInputBase>
    {
        public override ODataInputBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // ODataInputBase is only used for write operations (create/update), not for reading
            throw new NotSupportedException("Deserializing ODataInputBase is not supported. Use entity types for read operations.");
        }

        public override void Write(Utf8JsonWriter writer, ODataInputBase value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            // Get the dictionary representation of the input object
            var values = value.GetValues();

            // Write the JSON object directly
            writer.WriteStartObject();

            foreach (var kvp in values)
            {
                writer.WritePropertyName(kvp.Key);
                WriteValue(writer, kvp.Value, options);
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Writes a single value, recursively handling ODataInputBase objects and collections.
        /// </summary>
        private void WriteValue(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            // If it's an ODataInputBase, recursively write its GetValues()
            if (value is ODataInputBase inputBase)
            {
                Write(writer, inputBase, options);
                return;
            }

            // If it's a list/collection, write as array
            if (value is System.Collections.IEnumerable enumerable && value is not string)
            {
                // Use JsonSerializer to handle the collection with registered converters
                // (like ODataCollectionConverter for wrapping in "results")
                JsonSerializer.Serialize(writer, value, value.GetType(), options);
                return;
            }

            // For all other types, use JsonSerializer
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
