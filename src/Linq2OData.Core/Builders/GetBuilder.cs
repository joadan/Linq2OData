using System.Linq.Expressions;
using static System.Net.WebRequestMethods;

namespace Linq2OData.Core.Builders;




public class GetBuilder<T> where T : IODataEntitySet, new()
{

    private ODataClient odataClient;
    private string entityPath;
    private string keyExpression;

    public GetBuilder(ODataClient odataClient, Action<T> keySetter)
    {
        this.odataClient = odataClient;
        entityPath = BuilderHelper.GetEntityPath<T>();
        keyExpression = BuilderHelper.GetEntityKeys<T>(keySetter);
    }

    internal string? select;
    internal string? expand;

    internal ODataClient ODataClient => odataClient;
    internal  string EntityPath => entityPath;
    internal string KeyExpression => keyExpression;

    public async Task<T?> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var result = await odataClient.QueryEntityAsync<T>(EntityPath, keyExpression, select, expand, cancellationToken);
        if (result == null) { return default; }

        return result.Data;
    }

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

    public GetProjectionBuilder<T, TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
    {
        return new GetProjectionBuilder<T, TResult>(this, selector);
    }   



}
