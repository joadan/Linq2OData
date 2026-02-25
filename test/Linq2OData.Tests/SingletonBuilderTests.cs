using Linq2OData.Core;
using Linq2OData.Core.Builders;
using System.Net;

namespace Linq2OData.Tests;

/// <summary>
/// Tests for the SingletonBuilder class, which builds requests for OData singleton entities.
/// </summary>
public class SingletonBuilderTests
{
	[ODataEntitySet("Persons")]
	public class TestPerson : IODataEntitySet
	{
		[ODataMember("ID")]
		public int ID { get; set; }

		[ODataMember("Name")]
		public string? Name { get; set; }

		[ODataMember("Email")]
		public string? Email { get; set; }

		[ODataMember("Address", isComplex: true)]
		public TestAddress? Address { get; set; }

		public string __Key => $"ID={ID}";
	}

	[ODataEntitySet("Addresses")]
	public class TestAddress : IODataEntitySet
	{
		[ODataMember("City")]
		public string? City { get; set; }

		public string __Key => "";
	}

	private (SingletonBuilder<TestPerson> builder, MockHttpHandler handler) CreateBuilder(
		ODataVersion version = ODataVersion.V4,
		string responseBody = "{}",
		HttpStatusCode statusCode = HttpStatusCode.OK)
	{
		var handler = new MockHttpHandler(responseBody, statusCode);
		var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://example.com/odata/") };
		var odataClient = new ODataClient(httpClient, version);
		return (new SingletonBuilder<TestPerson>(odataClient, "Me"), handler);
	}

	#region Builder State Tests

	[Fact]
	public void SingletonBuilder_Select_SetsSelectField()
	{
		// Arrange
		var (builder, _) = CreateBuilder();

		// Act
		var result = builder.Select("Name,Email");

		// Assert
		Assert.Equal("Name,Email", builder.select);
		Assert.Same(builder, result);
	}

	[Fact]
	public void SingletonBuilder_Expand_SetsExpandField()
	{
		// Arrange
		var (builder, _) = CreateBuilder();

		// Act
		var result = builder.Expand("Address");

		// Assert
		Assert.Equal("Address", builder.expand);
		Assert.Same(builder, result);
	}

	[Fact]
	public void SingletonBuilder_SelectNull_SetsSelectFieldToNull()
	{
		// Arrange
		var (builder, _) = CreateBuilder();
		builder.Select("Name");

		// Act
		builder.Select(null);

		// Assert
		Assert.Null(builder.select);
	}

	[Fact]
	public void SingletonBuilder_ExpandNull_SetsExpandFieldToNull()
	{
		// Arrange
		var (builder, _) = CreateBuilder();
		builder.Expand("Address");

		// Act
		builder.Expand(null);

		// Assert
		Assert.Null(builder.expand);
	}

	[Fact]
	public void SingletonBuilder_ChainSelectAndExpand_SetsBothFields()
	{
		// Arrange
		var (builder, _) = CreateBuilder();

		// Act
		builder.Select("Name").Expand("Address");

		// Assert
		Assert.Equal("Name", builder.select);
		Assert.Equal("Address", builder.expand);
	}

	[Fact]
	public void SingletonBuilder_DefaultState_HasNullSelectAndExpand()
	{
		// Arrange / Act
		var (builder, _) = CreateBuilder();

		// Assert
		Assert.Null(builder.select);
		Assert.Null(builder.expand);
	}

	#endregion

	#region URL Generation Tests

	[Fact]
	public async Task SingletonBuilder_ExecuteAsync_NoSelectOrExpand_RequestsCorrectUrl()
	{
		// Arrange
		var json = """{"@odata.context":"https://example.com/odata/$metadata#Me","ID":1,"Name":"John"}""";
		var (builder, handler) = CreateBuilder(ODataVersion.V4, json);

		// Act
		await builder.ExecuteAsync();

		// Assert - singleton path with no query string (no key expression)
		Assert.Equal("/odata/Me", handler.LastRequestPath);
		Assert.Empty(handler.LastRequestQuery);
	}

	[Fact]
	public async Task SingletonBuilder_ExecuteAsync_WithSelect_IncludesSelectInUrl()
	{
		// Arrange
		var json = """{"@odata.context":"https://example.com/odata/$metadata#Me","Name":"John"}""";
		var (builder, handler) = CreateBuilder(ODataVersion.V4, json);
		builder.Select("Name,Email");

		// Act
		await builder.ExecuteAsync();

		// Assert
		Assert.Equal("/odata/Me", handler.LastRequestPath);
		Assert.Contains("$select=Name,Email", handler.LastRequestQuery);
	}

	[Fact]
	public async Task SingletonBuilder_ExecuteAsync_WithExpand_IncludesExpandInUrl()
	{
		// Arrange
		var json = """{"@odata.context":"...","Name":"John","Address":{"City":"NYC"}}""";
		var (builder, handler) = CreateBuilder(ODataVersion.V4, json);
		builder.Expand("Address");

		// Act
		await builder.ExecuteAsync();

		// Assert
		Assert.Equal("/odata/Me", handler.LastRequestPath);
		Assert.Contains("$expand=", handler.LastRequestQuery);
		Assert.Contains("Address", handler.LastRequestQuery);
	}

	[Fact]
	public async Task SingletonBuilder_ExecuteAsync_WithSelectAndExpand_IncludesBothInUrl()
	{
		// Arrange
		var json = """{"@odata.context":"...","Name":"John","Address":{"City":"NYC"}}""";
		var (builder, handler) = CreateBuilder(ODataVersion.V4, json);
		builder.Select("Name").Expand("Address");

		// Act
		await builder.ExecuteAsync();

		// Assert
		Assert.Equal("/odata/Me", handler.LastRequestPath);
		Assert.Contains("$select=Name", handler.LastRequestQuery);
		Assert.Contains("$expand=", handler.LastRequestQuery);
	}

	[Fact]
	public async Task SingletonBuilder_ExecuteAsync_DoesNotAppendKeyExpression()
	{
		// Arrange - singletons have no key, so the URL must NOT contain "(..."
		var json = """{"@odata.context":"...","ID":1,"Name":"John"}""";
		var (builder, handler) = CreateBuilder(ODataVersion.V4, json);

		// Act
		await builder.ExecuteAsync();

		// Assert - no parentheses for key
		Assert.DoesNotContain("(", handler.LastRequestPath);
	}

	#endregion

	#region Deserialization Tests

	[Fact]
	public async Task SingletonBuilder_ExecuteAsync_V4Response_DeserializesCorrectly()
	{
		// Arrange - OData V4 singleton response (entity at root, no "value" wrapper)
		var json = """{"@odata.context":"https://example.com/odata/$metadata#Me","ID":42,"Name":"Jane Doe","Email":"jane@example.com"}""";
		var (builder, _) = CreateBuilder(ODataVersion.V4, json);

		// Act
		var result = await builder.ExecuteAsync();

		// Assert
		Assert.NotNull(result);
		Assert.Equal(42, result.ID);
		Assert.Equal("Jane Doe", result.Name);
		Assert.Equal("jane@example.com", result.Email);
	}

	[Fact]
	public async Task SingletonBuilder_ExecuteAsync_V4ResponseWithExpand_DeserializesNestedEntity()
	{
		// Arrange
		var json = """{"@odata.context":"...","ID":1,"Name":"John","Address":{"City":"Seattle"}}""";
		var (builder, _) = CreateBuilder(ODataVersion.V4, json);
		builder.Expand("Address");

		// Act
		var result = await builder.ExecuteAsync();

		// Assert
		Assert.NotNull(result);
		Assert.Equal(1, result.ID);
		Assert.Equal("John", result.Name);
		Assert.NotNull(result.Address);
		Assert.Equal("Seattle", result.Address.City);
	}

	[Fact]
	public async Task SingletonBuilder_ExecuteAsync_NotFound_ReturnsNull()
	{
		// Arrange
		var (builder, _) = CreateBuilder(ODataVersion.V4, "", HttpStatusCode.NotFound);

		// Act
		var result = await builder.ExecuteAsync();

		// Assert
		Assert.Null(result);
	}

	#endregion

	/// <summary>
	/// A simple HTTP handler that captures request details and returns a fixed response.
	/// </summary>
	private class MockHttpHandler(string responseBody, HttpStatusCode statusCode = HttpStatusCode.OK) : HttpMessageHandler
	{
		public string LastRequestPath { get; private set; } = "";
		public string LastRequestQuery { get; private set; } = "";

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			LastRequestPath = request.RequestUri?.AbsolutePath ?? "";
			LastRequestQuery = request.RequestUri?.Query ?? "";

			return Task.FromResult(new HttpResponseMessage(statusCode)
			{
				Content = new StringContent(responseBody, System.Text.Encoding.UTF8, "application/json")
			});
		}
	}
}
