using Linq2OData.Core;
using System.Net;
using System.Text;

namespace Linq2OData.Tests;

public class UpdateEntityTests
{
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
		HttpStatusCode statusCode = HttpStatusCode.NoContent)
	{
		var capturedRequests = new List<HttpRequestMessage>();

		var handler = new MockHttpHandler(request =>
		{
			capturedRequests.Add(request);
			return new HttpResponseMessage(statusCode);
		});

		var httpClient = new HttpClient(handler)
		{
			BaseAddress = new Uri("https://example.com/odata/")
		};

		return (new ODataClient(httpClient, version), capturedRequests);
	}

	private class TestInput : ODataInputBase
	{
		public string? Name
		{
			get => GetValue<string>(nameof(Name));
			set => SetValue(nameof(Name), value);
		}
	}

	[Fact]
	public async Task UpdateEntityAsync_ShouldUsePatchMethod()
	{
		// Arrange
		var (client, capturedRequests) = CreateMockedClient(ODataVersion.V4);
		var input = new TestInput { Name = "Updated" };

		// Act
		await client.UpdateEntityAsync("Products", "1", input);

		// Assert
		Assert.Single(capturedRequests);
		Assert.Equal(HttpMethod.Patch, capturedRequests[0].Method);
	}

	[Fact]
	public async Task UpdateEntityAsync_ShouldReturnTrue_WhenSuccessful()
	{
		// Arrange
		var (client, _) = CreateMockedClient(ODataVersion.V4, HttpStatusCode.NoContent);
		var input = new TestInput { Name = "Updated" };

		// Act
		var result = await client.UpdateEntityAsync("Products", "1", input);

		// Assert
		Assert.True(result);
	}

	[Fact]
	public async Task UpdateEntityAsync_ShouldReturnFalse_WhenNotFound()
	{
		// Arrange
		var (client, _) = CreateMockedClient(ODataVersion.V4, HttpStatusCode.NotFound);
		var input = new TestInput { Name = "Updated" };

		// Act
		var result = await client.UpdateEntityAsync("Products", "1", input);

		// Assert
		Assert.False(result);
	}
}
