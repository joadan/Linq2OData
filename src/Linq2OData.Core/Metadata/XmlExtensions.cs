using System.Xml.Linq;

namespace Linq2OData.Core.Metadata;

internal static class XmlExtensions
{

    internal static string? GetAttributeValue(this XElement xElement, string attributeName, XNamespace? xnamespace = null)
    {

        var name = attributeName;

        if (xnamespace != null)
        {
            name = "{" + xnamespace.NamespaceName + "}" + attributeName;
        }

        var attribute = xElement.Attribute(name);
        return attribute?.Value;
    }

    internal static int? GetAttributeValueInt(this XElement xElement, string attributeName, XNamespace? xnamespace = null)
    {
        var value = xElement.GetAttributeValue(attributeName, xnamespace);

        if (int.TryParse(value, out int result))
        {
            return result;
        }

        return null;
    }

    internal static bool? GetAttributeValueBool(this XElement xElement, string attributeName, XNamespace? xnamespace = null)
    {
        var value = xElement.GetAttributeValue(attributeName, xnamespace);

        if (bool.TryParse(value, out bool result))
        {
            return result;
        }

        return null;
    }

    internal static ODataNavigationType GetNavigationType(this XElement xElement, string attributeName, XNamespace? xnamespace = null)
    {
        var value = xElement.GetAttributeValue(attributeName, xnamespace);

        if (value == "0..1")
        {
            return ODataNavigationType.ZeroOrOne;
        }
        else if (value == "1")
        {
            return ODataNavigationType.One;
        }
        else if (value == "*")
        {
            return ODataNavigationType.Many;
        }
        else
        {
            throw new InvalidOperationException($"Invalid navigation type value: {value}");
        }


    }

}
