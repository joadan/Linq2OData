using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linq2OData.Core.Converters
{
    /// <summary>
    /// Factory for creating ODataNavigationPropertyConverter instances for entity types.
    /// This handles the __deferred wrapper for non-expanded navigation properties in OData V2/V3.
    /// Only applies to types that implement IODataEntitySet to avoid infinite recursion.
    /// </summary>
    public class ODataNavigationPropertyConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            // Only convert types that implement IODataEntitySet (actual entity types)
            // This prevents the converter from being applied to nested complex types
            return typeof(IODataEntitySet).IsAssignableFrom(typeToConvert);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(ODataNavigationPropertyConverter<>).MakeGenericType(typeToConvert))!;

            return converter;
        }
    }
}
