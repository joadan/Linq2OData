using Linq2OData.Core.Converters;
using System.Text.Json;
using Xunit;

namespace Linq2OData.Tests
{
	public class DoubleStringJsonConverterTests
	{
		private static JsonSerializerOptions DoubleOptions()
		{
			var options = new JsonSerializerOptions();
			options.Converters.Add(new DoubleStringJsonConverter());
			return options;
		}

		private static JsonSerializerOptions NullableDoubleOptions()
		{
			var options = new JsonSerializerOptions();
			options.Converters.Add(new NullableDoubleStringJsonConverter());
			return options;
		}

		[Theory]
		[InlineData("\"3.14\"", 3.14)]
		[InlineData("\"1.5\"", 1.5)]
		[InlineData("\"0\"", 0.0)]
		[InlineData("\"-2.5\"", -2.5)]
		public void Read_StringValue_ShouldParseDouble(string json, double expected)
		{
			var result = JsonSerializer.Deserialize<double>(json, DoubleOptions());
			Assert.Equal(expected, result, 10);
		}

		[Theory]
		[InlineData("3.14", 3.14)]
		[InlineData("1.5", 1.5)]
		[InlineData("0", 0.0)]
		[InlineData("-2.5", -2.5)]
		public void Read_NumberValue_ShouldParseDouble(string json, double expected)
		{
			var result = JsonSerializer.Deserialize<double>(json, DoubleOptions());
			Assert.Equal(expected, result, 10);
		}

		[Fact]
		public void Write_ShouldSerializeAsString()
		{
			var value = 3.14;
			var json = JsonSerializer.Serialize(value, DoubleOptions());
			Assert.Equal("\"3.14\"", json);
		}

		[Fact]
		public void Write_NegativeValue_ShouldSerializeAsString()
		{
			var value = -1.5;
			var json = JsonSerializer.Serialize(value, DoubleOptions());
			Assert.Equal("\"-1.5\"", json);
		}

		[Theory]
		[InlineData("\"3.14\"", 3.14)]
		[InlineData("\"0\"", 0.0)]
		public void NullableRead_StringValue_ShouldParseDouble(string json, double expected)
		{
			var result = JsonSerializer.Deserialize<double?>(json, NullableDoubleOptions());
			Assert.NotNull(result);
			Assert.Equal(expected, result!.Value, 10);
		}

		[Fact]
		public void NullableRead_Null_ShouldReturnNull()
		{
			var result = JsonSerializer.Deserialize<double?>("null", NullableDoubleOptions());
			Assert.Null(result);
		}

		[Fact]
		public void NullableWrite_HasValue_ShouldSerializeAsString()
		{
			double? value = 1.23;
			var json = JsonSerializer.Serialize(value, NullableDoubleOptions());
			Assert.Equal("\"1.23\"", json);
		}

		[Fact]
		public void NullableWrite_Null_ShouldSerializeAsNull()
		{
			double? value = null;
			var json = JsonSerializer.Serialize(value, NullableDoubleOptions());
			Assert.Equal("null", json);
		}
	}
}
