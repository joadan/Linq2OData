using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Linq2OData.Client.ODataResponse
{
    public class ODataErrorResponseV1_3
    {
        [JsonPropertyName("error")]
        public ODataErrorV1_3? Error { get; set; }


    }

    public class ODataErrorV1_3
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }
        [JsonPropertyName("message")]
        public ODataErrorMessageV1_3? Message { get; set; }

        [JsonPropertyName("innererror")]
        public ODataInnerErrorV1_3? InnerError { get; set; }
    }

    public class ODataErrorMessageV1_3
    {
        [JsonPropertyName("lang")]
        public string? Lang { get; set; }
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class ODataInnerErrorV1_3
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("stacktrace")]
        public string? Stacktrace { get; set; }
    }

}
