using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linq2OData.Core.Converters
{
    /// <summary>
    /// Factory for creating ODataCollectionConverter instances for different list types.
    /// This allows the converter to work with List&lt;T&gt; where T is any entity type.
    /// Only used for OData V2/V3 where collections need to be wrapped in "results" object.
    /// </summary>
    public class ODataCollectionConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
            {
                return false;
            }

            var genericType = typeToConvert.GetGenericTypeDefinition();
            return genericType == typeof(List<>);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type elementType = typeToConvert.GetGenericArguments()[0];

            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(ODataCollectionConverter<>).MakeGenericType(elementType))!;

            return converter;
        }
    }
}
