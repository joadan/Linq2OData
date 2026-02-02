using Linq2OData.Core.Expressions;
using System.Linq.Expressions;

namespace Linq2OData.Core.Builders;

public class QueryBuilder<T> where T : IODataEntitySet, new()
{

    private ODataClient odataClient;
    private string entityPath;
    public QueryBuilder(ODataClient odataClient)
    {
        this.odataClient = odataClient;
        entityPath = BuilderHelper.GetEntityPath<T>();

    }

    internal bool count;
    internal int? top;
    internal int? skip;
    internal string? select;
    internal string? expand;
    internal string? filter;

    internal ODataClient ODataClient => odataClient;
    internal string EntityPath => entityPath;

    public QueryBuilder<T> Top(int? top)
    {
        this.top = top;
        return this;
    }

    public QueryBuilder<T> Count(bool count = true)
    {
        this.count = count;
        return this;
    }

    public QueryBuilder<T> Skip(int? skip)
    {
        this.skip = skip;
        return this;
    }

    public QueryBuilder<T> Expand(string? expand = null)
    {
        this.expand = expand;
        return this;
    }

    public QueryBuilder<T> Filter(string? filter = null)
    {
        this.filter = filter;
        return this;
    }

    public QueryBuilder<T> Filter(Expression<Func<T, bool>> expression)
    {
        var oDataFilterVisitor = new ODataFilterVisitor();
        this.filter = oDataFilterVisitor.ToFilter(expression);
        return this;
    }

    public async Task<List<T>?> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var result = await odataClient.QueryEntitySetAsync<T>(EntityPath, select, expand, filter, count, top, skip, cancellationToken);
        return result?.Data;
    }



    public QueryExecutor<T, List<T>> Select()
    {
        this.select = null;
        return new QueryExecutor<T, List<T>>(this, null);
    }

    public QueryExecutor<T, TResult> Select<TResult>(Expression<Func<List<T>, TResult>> selector)
    {
        //ParseExpression(selector);
        return new QueryExecutor<T, TResult>(this, selector);
    }
}
