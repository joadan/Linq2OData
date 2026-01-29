using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Linq2OData.Core.Converters;

internal static class MicrosoftDateTimeHelper
{
    internal static readonly Regex Regex =
        new Regex(@"^/Date\((\-?\d+)(?:[+-]\d{4})?\)/$",
                  RegexOptions.Compiled);

    internal static DateTime ParseDateTime(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new JsonException("Date value cannot be empty");

        var match = Regex.Match(value);
        if (!match.Success)
            throw new JsonException($"Invalid date format: {value}");

        var milliseconds = long.Parse(match.Groups[1].Value);

        // IMPORTANT: treat as UTC (matches old .NET behavior)
        return DateTimeOffset
            .FromUnixTimeMilliseconds(milliseconds)
            .UtcDateTime;
    }

    internal static string FormatDateTime(DateTime value)
    {
        // Normalize to UTC to avoid surprises
        var utc = value.Kind == DateTimeKind.Utc
            ? value
            : value.ToUniversalTime();

        ///Date(970358400000)/

     //   return @"/Date(1436832000000)/";
;

        var milliseconds = new DateTimeOffset(utc).ToUnixTimeMilliseconds();
        return $"/Date({milliseconds})/";
    }
}

public class MicrosoftDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {

        //If we get null we set min value rather then throw an exception
        if (reader.TokenType == JsonTokenType.Null)
            return DateTime.MinValue;

        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected string for DateTime");

        var value = reader.GetString()!;
        return MicrosoftDateTimeHelper.ParseDateTime(value);
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTime value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(MicrosoftDateTimeHelper.FormatDateTime(value));
    }
}

public class MicrosoftNullableDateTimeConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Expected string for DateTime");

        var value = reader.GetString()!;
        return MicrosoftDateTimeHelper.ParseDateTime(value);
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateTime? value,
        JsonSerializerOptions options)
    {
        if (!value.HasValue)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(MicrosoftDateTimeHelper.FormatDateTime(value.Value));
    }
}

