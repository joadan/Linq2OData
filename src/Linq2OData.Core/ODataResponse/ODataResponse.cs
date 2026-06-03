using System.Net;
using System.Text.Json.Serialization;

namespace Linq2OData.Core.ODataResponse;

public class ODataResponse<T>
{
    [JsonPropertyName("d")]
    public T? Data { get; set; }

    [JsonPropertyName("__count")]
    public long? Count { get; set; }

    /// <summary>
    /// The HTTP status code returned by the server.
    /// Populated when using <c>ExecuteResponseAsync</c> or the lower-level <c>ODataClient</c> methods.
    /// </summary>
    public HttpStatusCode? StatusCode { get; set; }

    /// <summary>
    /// Combined response and content headers returned by the server.
    /// Populated when using <c>ExecuteResponseAsync</c> or the lower-level <c>ODataClient</c> methods.
    /// Values are captured before the <see cref="HttpResponseMessage"/> is disposed.
    /// </summary>
    public IDictionary<string, IEnumerable<string>>? ResponseHeaders { get; set; }
}

