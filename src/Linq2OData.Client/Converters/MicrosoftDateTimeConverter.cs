using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Linq2OData.Client.Converters;

public class MicrosoftDateTimeConverter
    : JsonConverter<DateTime>
{
    private static readonly Regex _regex =
        new Regex(@"^/Date\((\-?\d+)(?:[+-]\d{4})?\)/$",
                  RegexOptions.Compiled);

    public override DateTime Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected string for DateTime");

        var value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
            throw new JsonException("Date value cannot be empty");

        var match = _regex.Match(value);
        if (!match.Success)
            throw new JsonException($"Invalid date format: {value}");

        var milliseconds = long.Parse(match.Groups[1].Value);

        // IMPORTANT: treat as UTC (matches old .NET behavior)
        return DateTimeOffset
            .FromUnixTimeMilliseconds(milliseconds)
            .UtcDateTime;
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTime value,
        JsonSerializerOptions options)
    {
        // Normalize to UTC to avoid surprises
        var utc = value.Kind == DateTimeKind.Utc
            ? value
            : value.ToUniversalTime();

        var milliseconds = new DateTimeOffset(utc).ToUnixTimeMilliseconds();
        writer.WriteStringValue($"/Date({milliseconds})/");
    }
}

