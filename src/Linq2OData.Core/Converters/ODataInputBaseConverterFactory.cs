using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linq2OData.Core.Converters
{
    /// <summary>
    /// Factory for creating ODataInputBaseConverter instances for ODataInputBase and its derived types.
    /// </summary>
    public class ODataInputBaseConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            // Check if the type is ODataInputBase or derives from it
            return typeof(ODataInputBase).IsAssignableFrom(typeToConvert);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return new ODataInputBaseConverter();
        }
    }
}
