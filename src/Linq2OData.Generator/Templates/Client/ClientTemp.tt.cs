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

        private static string GetActionParameterDict(ODataFunction func)
        {
            var parts = func.Parameters.Select(p => $"{{ \"{p.Name}\", {p.CSharpParameterName} }}");
            return string.Join(", ", parts);
        }

        private static string GetSingletonPath(ClientMetadata clientMetadata, ODataSingleton singleton)
        {
            if (string.IsNullOrWhiteSpace(clientMetadata.ServicePath))
                return singleton.Name;
            return $"{clientMetadata.ServicePath}/{singleton.Name}";
        }

        private IEnumerable<string> GetFunctionUsingDirectives()
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
                    foreach (var func in schema.Functions)
                    {
                        // Collect return type namespace
                        AddTypeNamespace(func.CSharpReturnType, func.ReturnType, typeToNsMap, namespaces);

                        // Collect parameter type namespaces
                        foreach (var param in func.Parameters)
                        {
                            if (!param.DataType.StartsWith("Edm."))
                            {
                                var typeName = param.DataType.Contains('.') ? param.DataType.Split('.').Last() : param.DataType;
                                if (typeToNsMap.TryGetValue(typeName, out var ns))
                                    namespaces.Add(ns);
                            }
                        }
                    }
                }
            }

            return namespaces.OrderBy(ns => ns);
        }

        private static void AddTypeNamespace(string csharpReturnType, string? rawReturnType, Dictionary<string, string> typeToNsMap, HashSet<string> namespaces)
        {
            if (string.IsNullOrEmpty(rawReturnType) || csharpReturnType == "void")
                return;

            var rt = rawReturnType;
            if (rt.StartsWith("Collection(") && rt.EndsWith(")"))
                rt = rt.Substring("Collection(".Length, rt.Length - "Collection(".Length - 1);

            if (rt.StartsWith("Edm."))
                return;

            var typeName = rt.Contains('.') ? rt.Split('.').Last() : rt;
            if (typeToNsMap.TryGetValue(typeName, out var ns))
                namespaces.Add(ns);
        }
    }
}
