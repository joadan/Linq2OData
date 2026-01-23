using System.Text.Json.Serialization;

namespace Linq2OData.Client
{
    public class ODataResponse<T>
    {
        // OData V2 wrapped payloads: { "d": { "results": [...] } }
        [JsonPropertyName("d")]
        public ODataData<T>? Data { get; set; }

        // Some services return arrays directly
        [JsonIgnore]
        public List<T> Results =>
            Data?.Results ?? new List<T>();
    }

    public class ODataData<T>
    {
        // Main entity collection
        [JsonPropertyName("results")]
        public List<T> Results { get; set; } = new();

        // Paging
        [JsonPropertyName("__next")]
        public string? NextLink { get; set; }
    }
}
