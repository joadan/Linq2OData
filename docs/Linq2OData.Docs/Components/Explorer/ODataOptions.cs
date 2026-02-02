using Linq2OData.Core.Metadata;

namespace Linq2OData.Docs.Components.Explorer
{
    public class ODataOptions
    {

        public string BaseUrl { get; set; } = string.Empty;
        public string? AuthToken { get; set; }

        public  ODataMetadata? odataMetadata { get; set; }


    }
}
