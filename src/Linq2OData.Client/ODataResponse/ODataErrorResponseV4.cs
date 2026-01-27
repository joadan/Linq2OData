using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Linq2OData.Client.ODataResponse
{
    public class ODataErrorResponseV4
    {
        [JsonPropertyName("error")]
        public ODataErrorV4? Error { get; set; }
    }

   
    public class ODataErrorV4
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("innererror")]
        public InnerErrorV4? InnerError { get; set; }
    }

    public class InnerErrorV4
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("stacktrace")]
        public string? Stacktrace { get; set; }
    }

   


}
