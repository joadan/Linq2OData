using Linq2OData.Core;
using Linq2OData.Core.Metadata;
using System.Text;

namespace Linq2OData.Generator.Templates.Types
{
    public partial class TypeTemplate(ODataEntityType entityType, string fullNamspace, string? servicePath, string etityInterfaceName, IEnumerable<ODataEntityType> derivedTypes, string metadataNamespace, ODataVersion odataVersion, IReadOnlyDictionary<string, string> typeToNsMap)
    {

        public string BaseTypeAndInterface
        {
            get
            {
                var result = string.IsNullOrWhiteSpace(entityType.BaseTypeCSharp) ? "" : $": {entityType.BaseTypeCSharp}";

                if (entityType.IsEntitySet)
                {
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        result = $" : {etityInterfaceName}";
                    }
                    else
                    {
                        result += $", {etityInterfaceName}";
                    }
                }

                if (entityType.KeyProperties.Any())
                {
                    if (string.IsNullOrWhiteSpace(result))
                    {
                        result = $" : {GetKeyInterface()}";
                    }
                    else
                    {
                        result += $", {GetKeyInterface()}";
                    }
                }

                return result;
            }
        }

        private string GetKeyInterface()
        {
            return $"I{entityType.Name}Keys";
        }

        private string GetEntitySetAttribute()
        {
            if (entityType.IsEntitySet)
            {
                if (string.IsNullOrWhiteSpace(servicePath))
                {
                    return $"[ODataEntitySet(\"{entityType.EntityPath}\")]";
                }


                return $"[ODataEntitySet(\"{servicePath}/{entityType.EntityPath}\")]";
            }

            return "";
        }

        private string GetEntitySetInterface()
        {
            if (entityType.IsEntitySet)
            {
                return $" : IODataEntitySet";
            }

            return "";
        }


        private string GetDerivedAttributes()
        {
            if (!derivedTypes.Any())
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[JsonPolymorphic(TypeDiscriminatorPropertyName = \"@odata.type\", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToBaseType)]");
            sb.AppendLine($"[JsonDerivedType(typeof({entityType.ClassName}))]");
            foreach (var derivedType in derivedTypes)
            {
                var typeNs = derivedType.SchemaNamespace ?? metadataNamespace;
                sb.AppendLine($"[JsonDerivedType(typeof({derivedType.ClassName}), \"#{typeNs}.{derivedType.Name}\")]");
            }
            return sb.ToString();
        }

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

            if (!string.IsNullOrEmpty(entityType.BaseType))
                referencedNames.Add(StripNs(entityType.BaseType));

            return referencedNames
                .Select(name => typeToNsMap.TryGetValue(name, out var ns) ? ns : null)
                .Where(ns => ns != null && ns != fullNamspace)
                .Distinct()
                .OrderBy(ns => ns)!;
        }

    }
}

