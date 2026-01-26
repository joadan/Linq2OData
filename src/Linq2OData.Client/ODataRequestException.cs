using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2OData.Client
{
    internal class ODataRequestException : SystemException
    {
        public ODataErrorResponse? ODataErrorResponse { get; private set; }

        public ODataRequestException(string message, ODataErrorResponse? oDataErrorResponse = null) : base(message)
        {
            ODataErrorResponse = oDataErrorResponse;
        }


       

    }
}
