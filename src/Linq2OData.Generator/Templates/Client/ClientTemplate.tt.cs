using Linq2OData.Core;
using Linq2OData.Core.Metadata;
using Linq2OData.Generator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2OData.Generator.Templates.Client;

public partial class ClientTemplate(string clientName, string clientNamespace, List<ODataMetadata> metadataList, ODataVersion oDataVersion)
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
}

