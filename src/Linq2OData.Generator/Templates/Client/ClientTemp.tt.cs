using Linq2OData.Core;
using Linq2OData.Core.Metadata;
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

        private static string GetSingletonPath(ClientMetadata clientMetadata, ODataSingleton singleton)
        {
            if (string.IsNullOrWhiteSpace(clientMetadata.ServicePath))
                return singleton.Name;
            return $"{clientMetadata.ServicePath}/{singleton.Name}";
        }

        private IEnumerable<string> GetSingletonUsingDirectives()
        {
            var namespaces = new HashSet<string>();

            foreach (var clientMetadata in request.Metadata)
            {
                var metadata = clientMetadata.Metadata;
                var typeToNsMap = metadata.Schemas
                    .SelectMany(s => s.EntityTypes.Select(et => (Schema: s, EntityType: et)))
                    .ToDictionary(
                        x => x.EntityType.Name,
                        x => request.Namespace + "." + (x.EntityType.SchemaNamespace ?? x.Schema.Namespace)
                    );

                foreach (var schema in metadata.Schemas)
                {
                    foreach (var singleton in schema.Singletons)
                    {
                        var typeName = singleton.EntityTypeName.Contains('.')
                            ? singleton.EntityTypeName.Split('.').Last()
                            : singleton.EntityTypeName;
                        if (typeToNsMap.TryGetValue(typeName, out var ns))
                            namespaces.Add(ns);
                    }
                }
            }

            return namespaces.OrderBy(ns => ns);
        }
    }
}
