namespace Linq2OData.Core.Builders;

public class GetBuilder<T>(ODataClient odataClient, string entityPath, string keyExpression)
{
    internal string? select;
    internal string? expand;

    internal ODataClient ODataClient => odataClient;
    internal string EntityPath => entityPath;
    internal string KeyExpression => keyExpression;

    public GetBuilder<T> Expand(string? expand = null)
    {
        this.expand = expand;
        return this;
    }
    public GetExecutor<T, T> Select(string? select = null)
    {
        this.select = select;
        return new GetExecutor<T, T>(this, null);
    }




}
