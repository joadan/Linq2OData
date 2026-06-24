using System.Text.Json;
using System.Text.Json.Serialization;

namespace Linq2OData.Core.Converters;

internal static class DoubleStringHelper
{
	internal static double ParseDouble(ref Utf8JsonReader reader)
	{
		return reader.TokenType switch
		{
			JsonTokenType.Number => reader.GetDouble(),
			JsonTokenType.String => double.Parse(reader.GetString()!, System.Globalization.CultureInfo.InvariantCulture),
			_ => throw new JsonException($"Unexpected token {reader.TokenType}")
		};
	}
}

public class DoubleStringJsonConverter : JsonConverter<double>
{
	public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return DoubleStringHelper.ParseDouble(ref reader);
	}

	public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
	{
		//OData 2.0 expects double values to be sent as strings
		writer.WriteStringValue(value.ToString(System.Globalization.CultureInfo.InvariantCulture));
	}
}

public class NullableDoubleStringJsonConverter : JsonConverter<double?>
{
	public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Null)
			return null;

		return DoubleStringHelper.ParseDouble(ref reader);
	}

	public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options)
	{
		if (!value.HasValue)
		{
			writer.WriteNullValue();
		}
		else
		{
			//OData 2.0 expects double values to be sent as strings
			writer.WriteStringValue(value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}
	}
}
