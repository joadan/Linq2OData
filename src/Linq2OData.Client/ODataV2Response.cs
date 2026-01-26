using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Linq2OData.Client;

public class ODataResponse<T>
{
    [JsonPropertyName("d")]
    public T? Data { get; set; }

    [JsonPropertyName("__count")]
    public long? Count { get; set; }

}

public class ODataErrorResponse
{
    [JsonPropertyName("error")]
    public ODataError? Error { get; set; }


}

public class ODataError
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    [JsonPropertyName("message")]
    public ODataErrorMessage? Message { get; set; }
  
    [JsonPropertyName("innererror")]
    public ODataInnerError? InnerError { get; set; }
}

public class ODataErrorMessage
{
    [JsonPropertyName("lang")]
    public string? Lang { get; set; }
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

public class ODataInnerError
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    [JsonPropertyName("stacktrace")]
    public string? Stacktrace { get; set; }
}
