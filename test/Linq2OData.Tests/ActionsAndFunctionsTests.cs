using Linq2OData.Core;
using System.Net;
using System.Text;

namespace Linq2OData.Tests;

/// <summary>
/// Tests for ODataClient Actions (POST) and Functions (GET) invocation,
/// covering URL construction, request body serialization, and response deserialization.
/// </summary>
public class ActionsAndFunctionsTests
{
	/// <summary>
	/// A minimal HttpMessageHandler that returns a pre-configured response
	/// and captures outgoing requests for inspection.
	/// </summary>
	private class MockHttpHandler : HttpMessageHandler
	{
		private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

		public MockHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
		{
			_handler = handler;
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return Task.FromResult(_handler(request));
		}
	}

	private static (ODataClient client, List<HttpRequestMessage> capturedRequests) CreateMockedClient(
		ODataVersion version,
		HttpStatusCode statusCode = HttpStatusCode.OK,
		string responseBody = "")
	{
		var capturedRequests = new List<HttpRequestMessage>();

		var handler = new MockHttpHandler(request =>
		{
			capturedRequests.Add(request);
			return new HttpResponseMessage(statusCode)
			{
				Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
			};
		});

		var httpClient = new HttpClient(handler)
		{
			BaseAddress = new Uri("https://example.com/odata/")
		};

		return (new ODataClient(httpClient, version), capturedRequests);
	}

	// Simple entity type used for deserialization assertions
	private class TestEntity : IODataEntitySet
	{
		public int ID { get; set; }
		public string? Name { get; set; }
		public string _Key => $"ID={ID}";
	}

	#region InvokeActionAsync (no return type)

	[Fact]
	public async Task InvokeActionAsync_ShouldUsePostMethod()
	{
		// Arrange
		var (client, requests) = CreateMockedClient(ODataVersion.V4, HttpStatusCode.NoContent);

		// Act
		await client.InvokeActionAsync("ResetDataSource");

		// Assert
		Assert.Single(requests);
		Assert.Equal(HttpMethod.Post, requests[0].Method);
	}

	[Fact]
	public async Task InvokeActionAsync_ShouldCallCorrectUrl()
	{
		// Arrange
		var (client, requests) = CreateMockedClient(ODataVersion.V4, HttpStatusCode.NoContent);

		// Act
		await client.InvokeActionAsync("ResetDataSource");

		// Assert
		Assert.Contains("ResetDataSource", requests[0].RequestUri!.ToString());
	}

	[Fact]
	public async Task InvokeActionAsync_WithNullParameters_ShouldSendEmptyJsonObject()
	{
		// Arrange
		var (client, requests) = CreateMockedClient(ODataVersion.V4, HttpStatusCode.NoContent);

		// Act
		await client.InvokeActionAsync("ResetDataSource", null);

		// Assert
		var body = await requests[0].Content!.ReadAsStringAsync();
		Assert.Equal("{}", body);
	}

	[Fact]
	public async Task InvokeActionAsync_WithParameters_ShouldSerializeParametersToBody()
	{
		// Arrange
		var (client, requests) = CreateMockedClient(ODataVersion.V4, HttpStatusCode.NoContent);
		var parameters = new Dictionary<string, object?> { { "rating", 5 }, { "comment", "good" } };

		// Act
		await client.InvokeActionAsync("RateProduct", parameters);

		// Assert
		var body = await requests[0].Content!.ReadAsStringAsync();
		Assert.Contains("rating", body);
		Assert.Contains("comment", body);
	}

	[Fact]
	public async Task InvokeActionAsync_WhenServerReturnsError_ShouldThrowODataRequestException()
	{
		// Arrange
		const string errorJson = @"{""error"":{""code"":""400"",""message"":""Bad Request""}}";
		var (client, _) = CreateMockedClient(ODataVersion.V4, HttpStatusCode.BadRequest, errorJson);

		// Act & Assert
		await Assert.ThrowsAsync<ODataRequestException>(
			() => client.InvokeActionAsync("MyAction"));
	}

	#endregion

	#region InvokeActionAsync<T> (with return type)

	[Fact]
	public async Task InvokeActionAsync_WithReturnType_ShouldUsePostMethod()
	{
		// Arrange
		const string responseJson = @"{""ID"":1,""Name"":""Test""}";
		var (client, requests) = CreateMockedClient(ODataVersion.V4, HttpStatusCode.OK, responseJson);

		// Act
		await client.InvokeActionAsync<TestEntity>("MyAction");

		// Assert
		Assert.Single(requests);
		Assert.Equal(HttpMethod.Post, requests[0].Method);
	}

	[Fact]
	public async Task InvokeActionAsync_WithReturnType_V4_ShouldDeserializeSingleEntityResponse()
	{
		// Arrange – V4 returns the entity object directly (no "d" wrapper)
		const string responseJson = @"{""ID"":42,""Name"":""TestResult""}";
		var (client, _) = CreateMockedClient(ODataVersion.V4, HttpStatusCode.OK, responseJson);

		// Act
		var result = await client.InvokeActionAsync<TestEntity>("MyAction");

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.Data);
		Assert.Equal(42, result.Data.ID);
		Assert.Equal("TestResult", result.Data.Name);
	}

	[Fact]
	public async Task InvokeActionAsync_WithReturnType_V2_ShouldDeserializeSingleEntityResponse()
	{
		// Arrange – V2 wraps the result in a "d" property
		const string responseJson = @"{""d"":{""ID"":42,""Name"":""TestResult""}}";
		var (client, _) = CreateMockedClient(ODataVersion.V2, HttpStatusCode.OK, responseJson);

		// Act
		var result = await client.InvokeActionAsync<TestEntity>("MyAction");

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.Data);
		Assert.Equal(42, result.Data.ID);
		Assert.Equal("TestResult", result.Data.Name);
	}

	[Fact]
	public async Task InvokeActionAsync_WithReturnType_WithParameters_ShouldSendParametersInBody()
	{
		// Arrange
		const string responseJson = @"{""ID"":1,""Name"":""Updated""}";
		var (client, requests) = CreateMockedClient(ODataVersion.V4, HttpStatusCode.OK, responseJson);
		var parameters = new Dictionary<string, object?> { { "newName", "Updated" } };

		// Act
		await client.InvokeActionAsync<TestEntity>("RenameEntity", parameters);

		// Assert
		var body = await requests[0].Content!.ReadAsStringAsync();
		Assert.Contains("newName", body);
	}

	[Fact]
	public async Task InvokeActionAsync_WithReturnType_WhenServerReturnsError_ShouldThrowODataRequestException()
	{
		// Arrange
		const string errorJson = @"{""error"":{""code"":""500"",""message"":""Server Error""}}";
		var (client, _) = CreateMockedClient(ODataVersion.V4, HttpStatusCode.InternalServerError, errorJson);

		// Act & Assert
		await Assert.ThrowsAsync<ODataRequestException>(
			() => client.InvokeActionAsync<TestEntity>("MyAction"));
	}

	#endregion

	#region InvokeFunctionAsync<T>

	[Fact]
	public async Task InvokeFunctionAsync_ShouldUseGetMethod()
	{
		// Arrange
		const string responseJson = @"{""ID"":1,""Name"":""Airport""}";
		var (client, requests) = CreateMockedClient(ODataVersion.V4, HttpStatusCode.OK, responseJson);

		// Act
		await client.InvokeFunctionAsync<TestEntity>("GetNearestAirport(lat=47.6,lon=-122.3)");

		// Assert
		Assert.Single(requests);
		Assert.Equal(HttpMethod.Get, requests[0].Method);
	}

	[Fact]
	public async Task InvokeFunctionAsync_ShouldCallCorrectUrl()
	{
		// Arrange
		const string responseJson = @"{""ID"":1,""Name"":""Airport""}";
		var (client, requests) = CreateMockedClient(ODataVersion.V4, HttpStatusCode.OK, responseJson);

		// Act
		await client.InvokeFunctionAsync<TestEntity>("GetNearestAirport(lat=47.6,lon=-122.3)");

		// Assert
		Assert.Contains("GetNearestAirport", requests[0].RequestUri!.ToString());
	}

	[Fact]
	public async Task InvokeFunctionAsync_V4_ShouldDeserializeSingleEntityResponse()
	{
		// Arrange – V4 single entity: direct JSON object
		const string responseJson = @"{""ID"":7,""Name"":""SeaTac""}";
		var (client, _) = CreateMockedClient(ODataVersion.V4, HttpStatusCode.OK, responseJson);

		// Act
		var result = await client.InvokeFunctionAsync<TestEntity>("GetNearestAirport(lat=47.6,lon=-122.3)");

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.Data);
		Assert.Equal(7, result.Data.ID);
		Assert.Equal("SeaTac", result.Data.Name);
	}

	[Fact]
	public async Task InvokeFunctionAsync_V4_ShouldDeserializeCollectionResponse()
	{
		// Arrange – V4 collection: { "value": [...] }
		const string responseJson = @"{""value"":[{""ID"":1,""Name"":""Item1""},{""ID"":2,""Name"":""Item2""}]}";
		var (client, _) = CreateMockedClient(ODataVersion.V4, HttpStatusCode.OK, responseJson);

		// Act
		var result = await client.InvokeFunctionAsync<List<TestEntity>>("GetTopItems");

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.Data);
		Assert.Equal(2, result.Data.Count);
		Assert.Equal(1, result.Data[0].ID);
		Assert.Equal("Item1", result.Data[0].Name);
		Assert.Equal(2, result.Data[1].ID);
	}

	[Fact]
	public async Task InvokeFunctionAsync_V2_ShouldDeserializeSingleEntityResponse()
	{
		// Arrange – V2 single entity: { "d": { ... } }
		const string responseJson = @"{""d"":{""ID"":3,""Name"":""Result""}}";
		var (client, _) = CreateMockedClient(ODataVersion.V2, HttpStatusCode.OK, responseJson);

		// Act
		var result = await client.InvokeFunctionAsync<TestEntity>("GetProductsByRating(rating=5)");

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.Data);
		Assert.Equal(3, result.Data.ID);
		Assert.Equal("Result", result.Data.Name);
	}

	[Fact]
	public async Task InvokeFunctionAsync_V2_ShouldDeserializeCollectionResponse()
	{
		// Arrange – V2 collection: { "d": { "results": [...] } }
		const string responseJson = @"{""d"":{""results"":[{""ID"":1,""Name"":""Item1""},{""ID"":2,""Name"":""Item2""}]}}";
		var (client, _) = CreateMockedClient(ODataVersion.V2, HttpStatusCode.OK, responseJson);

		// Act
		var result = await client.InvokeFunctionAsync<List<TestEntity>>("GetProductsByRating(rating=4)");

		// Assert
		Assert.NotNull(result);
		Assert.NotNull(result.Data);
		Assert.Equal(2, result.Data.Count);
		Assert.Equal(1, result.Data[0].ID);
		Assert.Equal("Item1", result.Data[0].Name);
	}

	[Fact]
	public async Task InvokeFunctionAsync_WhenNotFound_ShouldReturnNull()
	{
		// Arrange
		var (client, _) = CreateMockedClient(ODataVersion.V4, HttpStatusCode.NotFound, "");

		// Act
		var result = await client.InvokeFunctionAsync<TestEntity>("GetNearestAirport(lat=0.0,lon=0.0)");

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task InvokeFunctionAsync_WhenServerReturnsError_ShouldThrowODataRequestException()
	{
		// Arrange
		const string errorJson = @"{""error"":{""code"":""500"",""message"":""Internal Server Error""}}";
		var (client, _) = CreateMockedClient(ODataVersion.V4, HttpStatusCode.InternalServerError, errorJson);

		// Act & Assert
		await Assert.ThrowsAsync<ODataRequestException>(
			() => client.InvokeFunctionAsync<TestEntity>("MyFunction"));
	}

	#endregion
}
