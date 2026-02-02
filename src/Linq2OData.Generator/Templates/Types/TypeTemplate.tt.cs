using Linq2OData.Core.Metadata;
using System.Text;

namespace Linq2OData.Generator.Templates.Types
{
    public partial class TypeTemplate(ODataEntityType entityType, string fullNamspace, string etityInterfaceName, IEnumerable<ODataEntityType> derivedTypes, string metadataNamespace)
    {

        public string BaseTypeAndInterface
        {
            get
            {
                var result = string.IsNullOrWhiteSpace(entityType.BaseType) ? "" : $": {entityType.BaseType}";

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
                return $"[ODataEntitySet(\"{entityType.EntityPath}\")]";
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
            sb.AppendLine($"[JsonDerivedType(typeof({entityType.Name}))]");
            foreach (var derivedType in derivedTypes)
            {
                sb.AppendLine($"[JsonDerivedType(typeof({derivedType.Name}), \"#{metadataNamespace}.{derivedType.Name}\")]");
            }
            return sb.ToString();
        }

    }
}
