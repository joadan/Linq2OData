namespace Linq2OData.Core.Expressions;

public static class FilterHelper
{

    public static string ToODataFilter(DateTime? date, ODataVersion version, bool isOffset = false)
    {
        if (!date.HasValue) return "null";
        return ToODataFilter(date.Value, version, isOffset);
    }

    public static string ToODataFilter(DateTimeOffset? date, ODataVersion version)
    {
        if (!date.HasValue) return "null";
        return ToODataFilter(date.Value, version);
    }

    public static string ToODataFilter(DateTime date, ODataVersion version, bool isOffset = false)
    {
        //Note sure if this is correct, but for now we will ignore the version and just use the offset if requested.
        //Could be SAP Specific, but until we have a better understanding of the differences between V2 and V4, this is the best we can do.
        if (isOffset)
        {
            var dto = new DateTimeOffset(date);
            return $"datetimeoffset'{dto:yyyy-MM-ddTHH:mm:ssK}'";
        }
        else
        {
            return $"datetime'{date:yyyy-MM-ddTHH:mm:ss}'";
        }

        //if (version == ODataVersion.V2)
        //{
        //    //V2: ignores offset, always datetime'...' 
        //    return $"datetime'{date:yyyy-MM-ddTHH:mm:ss}'";
        //}
        //else // OData v4
        //{
        //    if (isOffset)
        //    {
        //        var dto = new DateTimeOffset(date);
        //        return $"datetimeoffset'{dto:yyyy-MM-ddTHH:mm:ssK}'";
        //    }
        //    else
        //    {
        //        return $"datetime'{date:yyyy-MM-ddTHH:mm:ss}'";
        //    }
        //}
    }

    public static string ToODataFilter(DateTimeOffset date, ODataVersion version)
    {
        if (version == ODataVersion.V2)
        {
            return $"datetime'{date:yyyy-MM-ddTHH:mm:ss}'";
        }
        else
        {
            return $"datetimeoffset'{date:yyyy-MM-ddTHH:mm:ssK}'";
        }
    }



}