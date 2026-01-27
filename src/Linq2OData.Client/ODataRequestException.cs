using Linq2OData.Client.ODataResponse;
using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2OData.Client
{
    internal class ODataRequestException : SystemException
    {
        public ODataError? ODataError { get; private set; }
        public string RequestUrl { get; private set; }
        public string RequestMethod { get; private set; }

        public ODataRequestException(string message, HttpRequestMessage? requestMessage, ODataError? odataError = null) : base(message)
        {
            RequestUrl = requestMessage?.RequestUri?.ToString() ?? "";
            RequestMethod = requestMessage?.Method.Method ?? "";
            ODataError = odataError;
        }


       

    }
}
