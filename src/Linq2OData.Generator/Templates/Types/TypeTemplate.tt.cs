using Linq2OData.Generator.Models;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace Linq2OData.Generator.Templates.Types
{
    public partial class TypeTemplate(ODataEntityType entityType, string namespaceName)
    {


        public string BaseTypeDerived => string.IsNullOrWhiteSpace(entityType.BaseType) ? "" : $": {entityType.BaseType}";

    }
}
