using Linq2OData.Core.Metadata;


namespace Linq2OData.Core
{
    public class ODataService
    {
        public required string Namespace { get; set; }
        public string? ServicePath { get; set; }

        public required ODataMetadata Metadata { get; set; }
    }
}
