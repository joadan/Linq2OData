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

        private string GetEntityAttribute()
        {
            if (!entityType.IsEntitySet)
                return "[ODataEntity]";
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


        private string GetPolymorphicMetadata()
        {
            if (!derivedTypes.Any())
            {
                return string.Empty;
            }

            // Generate ODataPolymorphic attribute with derived type mappings
            // Our custom converter will read this to handle polymorphism without STJ's [JsonPolymorphic]
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[ODataPolymorphic]");
            foreach (var derivedType in derivedTypes)
            {
                var typeNs = derivedType.SchemaNamespace ?? metadataNamespace;
                sb.AppendLine($"[ODataDerivedType(typeof({derivedType.ClassName}), \"#{typeNs}.{derivedType.Name}\")]");
            }
            // Also add the base type itself
            var baseTypeNs = entityType.SchemaNamespace ?? metadataNamespace;
            sb.AppendLine($"[ODataDerivedType(typeof({entityType.ClassName}), \"#{baseTypeNs}.{entityType.Name}\")]");
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

