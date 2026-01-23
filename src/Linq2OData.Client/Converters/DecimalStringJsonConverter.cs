using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linq2OData.Client.Converters;

public class DecimalStringJsonConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetDecimal(),
            JsonTokenType.String => decimal.Parse(reader.GetString()!, System.Globalization.CultureInfo.InvariantCulture),
            _ => throw new JsonException()
        };
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}
