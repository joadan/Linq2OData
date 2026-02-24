using Linq2OData.Core.Metadata;


namespace Linq2OData.Generator.Templates.Input
{
    public partial class InputTemplate(ODataEntityType entityType, string namespaceName, IReadOnlyDictionary<string, string> typeToNsMap)
    {
        private static string StripNs(string typeName)
            => typeName.Contains('.') ? typeName.Split('.').Last() : typeName;

        private IEnumerable<string> GetExternalNamespaces()
        {
            var referencedNames = new HashSet<string>();

            foreach (var nav in entityType.Navigations)
                referencedNames.Add(nav.ToEntity);

            foreach (var prop in entityType.Properties)
                if (!prop.DataType.StartsWith("Edm.") && !prop.IsEnumType)
                    referencedNames.Add(StripNs(prop.DataType));

            return referencedNames
                .Select(name => typeToNsMap.TryGetValue(name, out var ns) ? ns : null)
                .Where(ns => ns != null && ns != namespaceName)
                .Distinct()
                .OrderBy(ns => ns)!;
        }
    }
}
