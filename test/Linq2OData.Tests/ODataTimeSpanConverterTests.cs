using Linq2OData.Core.Converters;
using System.Text.Json;
using Xunit;

namespace Linq2OData.Tests
{
    public class ODataTimeSpanConverterTests
    {
        [Theory]
        [InlineData("PT1H30M", 1, 30, 0)] // ISO 8601 Duration: 1 hour 30 minutes
        [InlineData("PT2H", 2, 0, 0)] // ISO 8601 Duration: 2 hours
        [InlineData("PT15M", 0, 15, 0)] // ISO 8601 Duration: 15 minutes
        [InlineData("PT1H30M45S", 1, 30, 45)] // ISO 8601 Duration: 1 hour 30 minutes 45 seconds
        [InlineData("P1DT2H", 26, 0, 0)] // ISO 8601 Duration: 1 day 2 hours = 26 hours
        [InlineData("1:30:00", 1, 30, 0)] // Standard TimeSpan format
        [InlineData("02:00:00", 2, 0, 0)] // Standard TimeSpan format
        [InlineData("00:15:00", 0, 15, 0)] // Standard TimeSpan format
        public void Read_ShouldParseVariousTimeSpanFormats(string input, int expectedHours, int expectedMinutes, int expectedSeconds)
        {
            // Arrange
            var json = $"\"{input}\"";
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ODataTimeSpanConverter());

            // Act
            var result = JsonSerializer.Deserialize<TimeSpan>(json, options);

            // Assert
            Assert.Equal(TimeSpan.FromHours(expectedHours) + TimeSpan.FromMinutes(expectedMinutes) + TimeSpan.FromSeconds(expectedSeconds), result);
        }

        [Fact]
        public void Read_EmptyString_ShouldReturnZero()
        {
            // Arrange
            var json = "\"\"";
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ODataTimeSpanConverter());

            // Act
            var result = JsonSerializer.Deserialize<TimeSpan>(json, options);

            // Assert
            Assert.Equal(TimeSpan.Zero, result);
        }

        [Fact]
        public void Read_InvalidFormat_ShouldThrowJsonException()
        {
            // Arrange
            var json = "\"invalid\"";
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ODataTimeSpanConverter());

            // Act & Assert
            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<TimeSpan>(json, options));
        }

        [Fact]
        public void Write_ShouldWriteAsISO8601Duration()
        {
            // Arrange
            var timeSpan = new TimeSpan(1, 30, 0); // 1 hour 30 minutes
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ODataTimeSpanConverter());

            // Act
            var json = JsonSerializer.Serialize(timeSpan, options);

            // Assert
            // Should be ISO 8601 Duration format
            Assert.Contains("PT", json);
        }

        [Theory]
        [InlineData("PT1H30M", 1, 30, 0)]
        [InlineData("1:30:00", 1, 30, 0)]
        public void NullableRead_ShouldParseVariousTimeSpanFormats(string input, int expectedHours, int expectedMinutes, int expectedSeconds)
        {
            // Arrange
            var json = $"\"{input}\"";
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ODataNullableTimeSpanConverter());

            // Act
            var result = JsonSerializer.Deserialize<TimeSpan?>(json, options);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TimeSpan.FromHours(expectedHours) + TimeSpan.FromMinutes(expectedMinutes) + TimeSpan.FromSeconds(expectedSeconds), result.Value);
        }

        [Fact]
        public void NullableRead_Null_ShouldReturnNull()
        {
            // Arrange
            var json = "null";
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ODataNullableTimeSpanConverter());

            // Act
            var result = JsonSerializer.Deserialize<TimeSpan?>(json, options);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void NullableRead_EmptyString_ShouldReturnNull()
        {
            // Arrange
            var json = "\"\"";
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ODataNullableTimeSpanConverter());

            // Act
            var result = JsonSerializer.Deserialize<TimeSpan?>(json, options);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void NullableWrite_HasValue_ShouldWriteAsISO8601Duration()
        {
            // Arrange
            TimeSpan? timeSpan = new TimeSpan(1, 30, 0);
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ODataNullableTimeSpanConverter());

            // Act
            var json = JsonSerializer.Serialize(timeSpan, options);

            // Assert
            Assert.Contains("PT", json);
        }

        [Fact]
        public void NullableWrite_Null_ShouldWriteNull()
        {
            // Arrange
            TimeSpan? timeSpan = null;
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ODataNullableTimeSpanConverter());

            // Act
            var json = JsonSerializer.Serialize(timeSpan, options);

            // Assert
            Assert.Equal("null", json);
        }
    }
}
