using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Linq2OData.Client.Converters;

public class MicrosoftDateTimeOffsetConverter
    : JsonConverter<DateTimeOffset>
{
    private static readonly Regex _regex =
        new Regex(@"^/Date\((\-?\d+)(?:[+-]\d{4})?\)/$",
                  RegexOptions.Compiled);

    public override DateTimeOffset Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected string for DateTimeOffset");

        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
            throw new JsonException("Date value cannot be empty");

        var match = _regex.Match(value);
        if (!match.Success)
            throw new JsonException($"Invalid date format: {value}");

        var milliseconds = long.Parse(match.Groups[1].Value);
        return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTimeOffset value,
        JsonSerializerOptions options)
    {
        var milliseconds = value.ToUnixTimeMilliseconds();
        writer.WriteStringValue($"/Date({milliseconds})/");
    }
}
