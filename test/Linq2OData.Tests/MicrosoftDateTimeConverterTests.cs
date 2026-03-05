using Linq2OData.Core.Converters;
using System.Text.Json;
using Xunit;

namespace Linq2OData.Tests
{
	public class MicrosoftDateTimeConverterTests
	{
		private static JsonSerializerOptions DateTimeOptions()
		{
			var options = new JsonSerializerOptions();
			options.Converters.Add(new MicrosoftDateTimeConverter());
			return options;
		}

		private static JsonSerializerOptions NullableDateTimeOptions()
		{
			var options = new JsonSerializerOptions();
			options.Converters.Add(new MicrosoftNullableDateTimeConverter());
			return options;
		}

		private static JsonSerializerOptions DateTimeOffsetOptions()
		{
			var options = new JsonSerializerOptions();
			options.Converters.Add(new MicrosoftDateTimeOffsetConverter());
			return options;
		}

		[Fact]
		public void Read_NormalValue_ShouldParse()
		{
			var json = "\"/Date(970358400000)/\"";
			var result = JsonSerializer.Deserialize<DateTime>(json, DateTimeOptions());
			Assert.Equal(new DateTime(2000, 10, 1, 0, 0, 0, DateTimeKind.Utc), result);
		}

		[Fact]
		public void Read_BelowMinRange_ShouldReturnMinValue()
		{
			// -62135769600000 is below the valid minimum of -62135596800000
			var json = "\"/Date(-62135769600000)/\"";
			var result = JsonSerializer.Deserialize<DateTime>(json, DateTimeOptions());
			Assert.Equal(DateTime.MinValue, result);
		}

		[Fact]
		public void Read_AboveMaxRange_ShouldReturnMaxValue()
		{
			// 253402300800000 is above the valid maximum of 253402300799999
			var json = "\"/Date(253402300800000)/\"";
			var result = JsonSerializer.Deserialize<DateTime>(json, DateTimeOptions());
			Assert.Equal(DateTime.MaxValue, result);
		}

		[Fact]
		public void Read_ExactMinRange_ShouldParse()
		{
			var json = "\"/Date(-62135596800000)/\"";
			var result = JsonSerializer.Deserialize<DateTime>(json, DateTimeOptions());
			Assert.Equal(DateTime.MinValue.Date, result.Date);
		}

		[Fact]
		public void NullableRead_BelowMinRange_ShouldReturnMinValue()
		{
			var json = "\"/Date(-62135769600000)/\"";
			var result = JsonSerializer.Deserialize<DateTime?>(json, NullableDateTimeOptions());
			Assert.Equal(DateTime.MinValue, result);
		}

		[Fact]
		public void NullableRead_AboveMaxRange_ShouldReturnMaxValue()
		{
			var json = "\"/Date(253402300800000)/\"";
			var result = JsonSerializer.Deserialize<DateTime?>(json, NullableDateTimeOptions());
			Assert.Equal(DateTime.MaxValue, result);
		}

		[Fact]
		public void DateTimeOffset_Read_BelowMinRange_ShouldReturnMinValue()
		{
			var json = "\"/Date(-62135769600000)/\"";
			var result = JsonSerializer.Deserialize<DateTimeOffset>(json, DateTimeOffsetOptions());
			Assert.Equal(DateTimeOffset.MinValue, result);
		}

		[Fact]
		public void DateTimeOffset_Read_AboveMaxRange_ShouldReturnMaxValue()
		{
			var json = "\"/Date(253402300800000)/\"";
			var result = JsonSerializer.Deserialize<DateTimeOffset>(json, DateTimeOffsetOptions());
			Assert.Equal(DateTimeOffset.MaxValue, result);
		}

		[Fact]
		public void DateTimeOffset_Read_NormalValue_ShouldParse()
		{
			var json = "\"/Date(970358400000)/\"";
			var result = JsonSerializer.Deserialize<DateTimeOffset>(json, DateTimeOffsetOptions());
			Assert.Equal(new DateTimeOffset(2000, 10, 1, 0, 0, 0, TimeSpan.Zero), result);
		}
	}
}
