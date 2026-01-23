using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linq2OData.Client.Converters;

public class Int32StringJsonConverter : JsonConverter<int>
{
    public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetInt32(),
            JsonTokenType.String => int.Parse(reader.GetString()!),
            _ => throw new JsonException($"Unexpected token {reader.TokenType}")
        };
    }

    public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}

