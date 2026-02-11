

using Linq2OData.Core.Metadata;

namespace Linq2OData.Generator.Models;

public class ClientRequest
{
    public required string Name { get; set; }
    public required string Namespace { get; set; }

    public List<ClientMetadata> Metadata { get; set; } = [];


    public ClientMetadata AddMetadata(string metadata,  string? path = null)
    {
        
        var clientMetadata = new ClientMetadata
        {
            ServicePath = path,
            Metadata =  MetadataParser.Parse(metadata)
        };



        Metadata.Add(clientMetadata);
        return clientMetadata;
    }

    public ClientMetadata AddMetadata(ODataMetadata metadata, string? path = null)
    {
        var clientMetadata = new ClientMetadata
        {
            ServicePath = path,
            Metadata = metadata
        };
        Metadata.Add(clientMetadata);
        return clientMetadata;
    }

}

public class ClientMetadata
{
    public string? ServicePath { get; set; }
    public required ODataMetadata Metadata { get; set; }
}
