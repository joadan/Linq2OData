using Linq2OData.Core.Metadata;
using Microsoft.AspNetCore.Components;

namespace Linq2OData.Docs.Components.Explorer
{
    public partial class TypeView
    {

        [Parameter] public ODataEntityType? ODataType { get; set; }

    }
}