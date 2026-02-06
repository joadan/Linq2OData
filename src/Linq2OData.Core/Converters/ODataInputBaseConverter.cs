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

            // Serialize the dictionary, which will recursively handle nested ODataInputBase objects
            JsonSerializer.Serialize(writer, values, options);
        }
    }
}
