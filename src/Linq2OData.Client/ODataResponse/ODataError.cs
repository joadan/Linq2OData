using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2OData.Client.ODataResponse
{
    public class ODataError
    {
        public string? Message { get; set; }
        public string? Code { get; set; }
        public ODataInnerError? InnerError { get; set; }
    }

    public class ODataInnerError
    {
        public string? Message { get; set; }
        public string? Type { get; set; }
        public string? Stacktrace { get; set; }
    }

}
