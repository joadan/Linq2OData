using Linq2OData.Core;
using Linq2OData.Generator.Models;


namespace Linq2OData.Generator.Templates.Client
{
    public partial class ClientTemp(ClientRequest request, ODataVersion oDataVersion)
    {

        private string GetODataVersionParameter()
        {
            return oDataVersion switch
            {
                ODataVersion.V2 => "Linq2OData.Core.ODataVersion.V2",
                ODataVersion.V4 => "Linq2OData.Core.ODataVersion.V4",
                _ => ""
            };
        }
    }
}
