using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Linq2OData.Client.ODataResponse;

public class ODataResponse<T>
{
    [JsonPropertyName("d")]
    public T? Data { get; set; }

    [JsonPropertyName("__count")]
    public long? Count { get; set; }

}

