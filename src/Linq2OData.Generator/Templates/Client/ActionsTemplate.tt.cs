using Linq2OData.Core;
using Linq2OData.Core.Metadata;
using Linq2OData.Generator.Models;

namespace Linq2OData.Generator.Templates.Client
{
    public partial class ActionsTemplate(ClientRequest request, ClientMetadata clientMetadata, ODataVersion oDataVersion)
    {
        private static string GetActionParameterDict(ODataFunction func)
        {
            var parts = func.Parameters.Select(p => $"{{ \"{p.Name}\", {p.CSharpParameterName} }}");
            return string.Join(", ", parts);
        }

        private IEnumerable<string> GetUsingDirectives()
        {
            var namespaces = new HashSet<string>();

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
                    AddTypeNamespace(func.CSharpReturnType, func.ReturnType, typeToNsMap, namespaces);

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
