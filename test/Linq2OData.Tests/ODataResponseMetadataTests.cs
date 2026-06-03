using Linq2OData.Core;
using Linq2OData.Core.Builders;
using Linq2OData.Core.ODataResponse;
using System.Net;
using System.Text;

namespace Linq2OData.Tests;

/// <summary>
/// Tests that ODataResponse.StatusCode and ODataResponse.ResponseHeaders are populated
/// by the various ODataClient execution paths.
/// </summary>
public class ODataResponseMetadataTests
{
	// -----------------------------------------------------------------------
	// Infrastructure
	// -----------------------------------------------------------------------

	private class MockHttpHandler : HttpMessageHandler
	{
		private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

		public MockHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
			=> _handler = handler;

		protected override Task<HttpResponseMessage> SendAsync(
			HttpRequestMessage request, CancellationToken cancellationToken)
			=> Task.FromResult(_handler(request));
	}

	private static ODataClient CreateClient(
		ODataVersion version,
		HttpStatusCode statusCode,
		string responseBody,
		Dictionary<string, string>? customHeaders = null)
	{
		var handler = new MockHttpHandler(req =>
		{
			var resp = new HttpResponseMessage(statusCode)
			{
				Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
			};
			if (customHeaders != null)
			{
				foreach (var h in customHeaders)
					resp.Headers.TryAddWithoutValidation(h.Key, h.Value);
			}
			return resp;
		});

		var httpClient = new HttpClient(handler)
		{
			BaseAddress = new Uri("https://example.com/odata/")
		};

		return new ODataClient(httpClient, version);
	}

	[ODataEntitySet("TestEntities")]
	private class TestEntity : IODataEntitySet
	{
		public int ID { get; set; }
		public string? Name { get; set; }
		public string _Key => $"ID={ID}";
	}

	private class TestInput : ODataInputBase
	{
		public string? Name { get; set; }
	}

	// -----------------------------------------------------------------------
	// Builder ExecuteResponseAsync methods
	// -----------------------------------------------------------------------

	[Fact]
	public async Task QueryBuilder_ExecuteResponseAsync_ShouldPopulateStatusCodeAndHeaders()
	{
		const string json = @"{""value"":[{""ID"":1,""Name"":""A""}]}";
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.OK, json,
			new Dictionary<string, string> { { "X-Query-Header", "query-val" } });
		var builder = new QueryBuilder<TestEntity>(client);

		var result = await builder.ExecuteResponseAsync();

		Assert.Equal(HttpStatusCode.OK, result.StatusCode);
		Assert.NotNull(result.ResponseHeaders);
		Assert.True(result.ResponseHeaders!.ContainsKey("X-Query-Header"));
	}

	[Fact]
	public async Task GetBuilder_ExecuteResponseAsync_ShouldPopulateStatusCodeAndHeaders()
	{
		const string json = @"{""ID"":1,""Name"":""A""}";
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.OK, json,
			new Dictionary<string, string> { { "ETag", "\"e1\"" } });
		var builder = new GetBuilder<TestEntity>(client, x => x.ID = 1);

		var result = await builder.ExecuteResponseAsync();

		Assert.NotNull(result);
		Assert.Equal(HttpStatusCode.OK, result!.StatusCode);
		Assert.NotNull(result.ResponseHeaders);
		Assert.True(result.ResponseHeaders!.ContainsKey("ETag"));
	}

	[Fact]
	public async Task SingletonBuilder_ExecuteResponseAsync_ShouldPopulateStatusCodeAndHeaders()
	{
		const string json = @"{""ID"":1,""Name"":""A""}";
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.OK, json,
			new Dictionary<string, string> { { "X-Singleton-Header", "singleton-val" } });
		var builder = new SingletonBuilder<TestEntity>(client, "Me");

		var result = await builder.ExecuteResponseAsync();

		Assert.NotNull(result);
		Assert.Equal(HttpStatusCode.OK, result!.StatusCode);
		Assert.NotNull(result.ResponseHeaders);
		Assert.True(result.ResponseHeaders!.ContainsKey("X-Singleton-Header"));
	}

	[Fact]
	public async Task CreateBuilder_ExecuteResponseAsync_ShouldPopulateStatusCodeAndHeaders()
	{
		const string json = @"{""ID"":99,""Name"":""Created""}";
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.Created, json,
			new Dictionary<string, string> { { "Location", "https://example.com/odata/TestEntities(99)" } });
		var builder = new CreateBuilder<TestEntity>(client);

		var result = await builder.ExecuteResponseAsync(new TestInput { Name = "Created" });

		Assert.Equal(HttpStatusCode.Created, result.StatusCode);
		Assert.NotNull(result.ResponseHeaders);
		Assert.True(result.ResponseHeaders!.ContainsKey("Location"));
	}

	[Fact]
	public async Task UpdateBuilder_ExecuteResponseAsync_ShouldPopulateStatusCodeAndHeaders()
	{
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.NoContent, string.Empty,
			new Dictionary<string, string> { { "X-Update-Header", "update-val" } });
		var builder = new UpdateBuilder<TestEntity>(client, x => x.ID = 1);

		var result = await builder.ExecuteResponseAsync(new TestInput { Name = "Updated" });

		Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
		Assert.True(result.Data);
		Assert.NotNull(result.ResponseHeaders);
		Assert.True(result.ResponseHeaders!.ContainsKey("X-Update-Header"));
	}

	[Fact]
	public async Task DeleteBuilder_ExecuteResponseAsync_ShouldPopulateStatusCodeAndHeaders()
	{
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.NoContent, string.Empty,
			new Dictionary<string, string> { { "X-Delete-Header", "delete-val" } });
		var builder = new DeleteBuilder<TestEntity>(client, x => x.ID = 1);

		var result = await builder.ExecuteResponseAsync();

		Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
		Assert.True(result.Data);
		Assert.NotNull(result.ResponseHeaders);
		Assert.True(result.ResponseHeaders!.ContainsKey("X-Delete-Header"));
	}

	[Fact]
	public async Task OrderByBuilder_ExecuteResponseAsync_ShouldPopulateStatusCodeAndHeaders()
	{
		const string json = @"{""value"":[{""ID"":1,""Name"":""A""}]}";
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.OK, json,
			new Dictionary<string, string> { { "X-OrderBy-Header", "orderby-val" } });
		var queryBuilder = new QueryBuilder<TestEntity>(client);
		var builder = queryBuilder.OrderBy(x => x.ID);

		var result = await builder.ExecuteResponseAsync();

		Assert.Equal(HttpStatusCode.OK, result.StatusCode);
		Assert.NotNull(result.ResponseHeaders);
		Assert.True(result.ResponseHeaders!.ContainsKey("X-OrderBy-Header"));
	}

	// -----------------------------------------------------------------------
	// QueryEntitySetAsync
	// -----------------------------------------------------------------------

	[Fact]
	public async Task QueryEntitySetAsync_ShouldPopulateStatusCode()
	{
		const string json = @"{""value"":[{""ID"":1,""Name"":""A""}]}";
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.OK, json);

		var result = await client.QueryEntitySetAsync<TestEntity>("TestEntities", null);

		Assert.Equal(HttpStatusCode.OK, result.StatusCode);
	}

	[Fact]
	public async Task QueryEntitySetAsync_ShouldPopulateResponseHeaders()
	{
		const string json = @"{""value"":[]}";
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.OK, json,
			new Dictionary<string, string> { { "X-Custom-Header", "custom-value" } });

		var result = await client.QueryEntitySetAsync<TestEntity>("TestEntities", null);

		Assert.NotNull(result.ResponseHeaders);
		Assert.True(result.ResponseHeaders!.ContainsKey("X-Custom-Header"));
		Assert.Contains("custom-value", result.ResponseHeaders["X-Custom-Header"]);
	}

	// -----------------------------------------------------------------------
	// QueryEntityAsync
	// -----------------------------------------------------------------------

	[Fact]
	public async Task QueryEntityAsync_ShouldPopulateStatusCode()
	{
		const string json = @"{""ID"":42,""Name"":""Test""}";
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.OK, json);

		var result = await client.QueryEntityAsync<TestEntity>("TestEntities", "ID=42");

		Assert.NotNull(result);
		Assert.Equal(HttpStatusCode.OK, result!.StatusCode);
	}

	[Fact]
	public async Task QueryEntityAsync_ShouldPopulateResponseHeaders()
	{
		const string json = @"{""ID"":1,""Name"":""A""}";
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.OK, json,
			new Dictionary<string, string> { { "ETag", "\"abc123\"" } });

		var result = await client.QueryEntityAsync<TestEntity>("TestEntities", "ID=1");

		Assert.NotNull(result?.ResponseHeaders);
		Assert.True(result!.ResponseHeaders!.ContainsKey("ETag"));
	}

	// -----------------------------------------------------------------------
	// CreateEntityWithResponseAsync
	// -----------------------------------------------------------------------

	[Fact]
	public async Task CreateEntityWithResponseAsync_ShouldPopulateStatusCode()
	{
		const string json = @"{""ID"":99,""Name"":""New""}";
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.Created, json);

		var result = await client.CreateEntityWithResponseAsync<TestEntity>(
			"TestEntities", new TestInput { Name = "New" });

		Assert.Equal(HttpStatusCode.Created, result.StatusCode);
		Assert.NotNull(result.Data);
		Assert.Equal(99, result.Data!.ID);
	}

	[Fact]
	public async Task CreateEntityWithResponseAsync_ShouldPopulateResponseHeaders()
	{
		const string json = @"{""ID"":1,""Name"":""Created""}";
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.Created, json,
			new Dictionary<string, string> { { "Location", "https://example.com/odata/TestEntities(1)" } });

		var result = await client.CreateEntityWithResponseAsync<TestEntity>(
			"TestEntities", new TestInput { Name = "Created" });

		Assert.NotNull(result.ResponseHeaders);
		Assert.True(result.ResponseHeaders!.ContainsKey("Location"));
	}

	// -----------------------------------------------------------------------
	// UpdateEntityWithResponseAsync
	// -----------------------------------------------------------------------

	[Fact]
	public async Task UpdateEntityWithResponseAsync_ShouldPopulateStatusCodeAndReturnTrue()
	{
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.NoContent, string.Empty);

		var result = await client.UpdateEntityWithResponseAsync(
			"TestEntities", "ID=1", new TestInput { Name = "Updated" });

		Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
		Assert.True(result.Data);
	}

	[Fact]
	public async Task UpdateEntityWithResponseAsync_WhenNotFound_ShouldPopulateStatusCodeAndReturnFalse()
	{
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.NotFound, string.Empty);

		var result = await client.UpdateEntityWithResponseAsync(
			"TestEntities", "ID=999", new TestInput { Name = "X" });

		Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
		Assert.False(result.Data);
	}

	[Fact]
	public async Task UpdateEntityWithResponseAsync_ShouldPopulateResponseHeaders()
	{
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.NoContent, string.Empty,
			new Dictionary<string, string> { { "X-Request-Id", "req-42" } });

		var result = await client.UpdateEntityWithResponseAsync(
			"TestEntities", "ID=1", new TestInput { Name = "U" });

		Assert.NotNull(result.ResponseHeaders);
		Assert.True(result.ResponseHeaders!.ContainsKey("X-Request-Id"));
	}

	// -----------------------------------------------------------------------
	// DeleteEntityWithResponseAsync
	// -----------------------------------------------------------------------

	[Fact]
	public async Task DeleteEntityWithResponseAsync_ShouldPopulateStatusCodeAndReturnTrue()
	{
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.NoContent, string.Empty);

		var result = await client.DeleteEntityWithResponseAsync("TestEntities", "ID=1");

		Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
		Assert.True(result.Data);
	}

	[Fact]
	public async Task DeleteEntityWithResponseAsync_WhenNotFound_ShouldPopulateStatusCodeAndReturnFalse()
	{
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.NotFound, string.Empty);

		var result = await client.DeleteEntityWithResponseAsync("TestEntities", "ID=999");

		Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
		Assert.False(result.Data);
	}

	[Fact]
	public async Task DeleteEntityWithResponseAsync_ShouldPopulateResponseHeaders()
	{
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.NoContent, string.Empty,
			new Dictionary<string, string> { { "X-Correlation-Id", "corr-7" } });

		var result = await client.DeleteEntityWithResponseAsync("TestEntities", "ID=1");

		Assert.NotNull(result.ResponseHeaders);
		Assert.True(result.ResponseHeaders!.ContainsKey("X-Correlation-Id"));
	}

	// -----------------------------------------------------------------------
	// InvokeActionAsync<T>
	// -----------------------------------------------------------------------

	[Fact]
	public async Task InvokeActionAsync_WithReturnType_ShouldPopulateStatusCode()
	{
		const string json = @"{""ID"":5,""Name"":""Result""}";
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.OK, json);

		var result = await client.InvokeActionAsync<TestEntity>("MyAction");

		Assert.NotNull(result);
		Assert.Equal(HttpStatusCode.OK, result!.StatusCode);
	}

	[Fact]
	public async Task InvokeActionAsync_WithReturnType_ShouldPopulateResponseHeaders()
	{
		const string json = @"{""ID"":1,""Name"":""R""}";
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.OK, json,
			new Dictionary<string, string> { { "X-Action-Header", "action-val" } });

		var result = await client.InvokeActionAsync<TestEntity>("MyAction");

		Assert.NotNull(result?.ResponseHeaders);
		Assert.True(result!.ResponseHeaders!.ContainsKey("X-Action-Header"));
	}

	// -----------------------------------------------------------------------
	// InvokeFunctionAsync<T>
	// -----------------------------------------------------------------------

	[Fact]
	public async Task InvokeFunctionAsync_ShouldPopulateStatusCode()
	{
		const string json = @"{""ID"":3,""Name"":""Airport""}";
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.OK, json);

		var result = await client.InvokeFunctionAsync<TestEntity>("GetNearest(lat=1,lon=2)");

		Assert.NotNull(result);
		Assert.Equal(HttpStatusCode.OK, result!.StatusCode);
	}

	[Fact]
	public async Task InvokeFunctionAsync_ShouldPopulateResponseHeaders()
	{
		const string json = @"{""ID"":1,""Name"":""R""}";
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.OK, json,
			new Dictionary<string, string> { { "X-Function-Header", "fn-val" } });

		var result = await client.InvokeFunctionAsync<TestEntity>("MyFunction");

		Assert.NotNull(result?.ResponseHeaders);
		Assert.True(result!.ResponseHeaders!.ContainsKey("X-Function-Header"));
	}

	// -----------------------------------------------------------------------
	// ContentHeaders are also copied
	// -----------------------------------------------------------------------

	[Fact]
	public async Task QueryEntitySetAsync_ShouldIncludeContentTypeInResponseHeaders()
	{
		const string json = @"{""value"":[]}";
		var client = CreateClient(ODataVersion.V4, HttpStatusCode.OK, json);

		var result = await client.QueryEntitySetAsync<TestEntity>("TestEntities", null);

		Assert.NotNull(result.ResponseHeaders);
		// Content-Type is a content header and should be merged into ResponseHeaders
		Assert.True(result.ResponseHeaders!.ContainsKey("Content-Type"));
	}
}
