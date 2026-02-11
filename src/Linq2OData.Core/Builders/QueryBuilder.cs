using Linq2OData.Core.Expressions;
using System.Linq.Expressions;

namespace Linq2OData.Core.Builders;

public class QueryBuilder<T> where T : IODataEntitySet, new()
{

    private ODataClient odataClient;
    private string entityPath;
    private List<string> expandPaths = new();

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
    internal string? orderby;

    internal ODataClient ODataClient => odataClient;
    internal string EntityPath => entityPath;

    internal QueryNode? queryNode;

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
        this.expandPaths.Clear(); // Clear the list when using string-based expand
        return this;
    }

    internal QueryNode MergeExpression<TResult>(Expression<Func<List<T>, TResult>> selector)
    {
        var visitor = new QueryNodeVisitor();
        var node = visitor.Parse(selector);
        if (queryNode == null)
        {
            queryNode = node;
        }
        else
        {
            queryNode.AddMergeChildren(node);
        }

        return queryNode;
    }


    public QueryBuilder<T> Expand<TResult>(Expression<Func<List<T>, TResult>> selector)
    {
         expand = MergeExpression(selector).GetOnlyExpand(odataClient.ODataVersion);
        return this;
    }



    /// <summary>
    /// Internal method to update the last expand path from ExpandBuilder (for nested expands).
    /// </summary>
    internal void UpdateLastExpandPath(string updatedPath)
    {
        if (expandPaths.Count > 0)
        {
            expandPaths[expandPaths.Count - 1] = updatedPath;
            this.expand = string.Join(",", expandPaths);
        }
    }

    public QueryBuilder<T> Filter(string? filter = null)
    {
        this.filter = filter;
        return this;
    }

    public QueryBuilder<T> Filter(Expression<Func<T, bool>> expression)
    {
        var oDataFilterVisitor = new ODataFilterVisitor();
        this.filter = oDataFilterVisitor.ToFilter(expression, odataClient.ODataVersion);
        return this;
    }

    /// <summary>
    /// Orders the results by a property in ascending order using a string.
    /// </summary>
    public QueryBuilder<T> Order(string? orderby = null)
    {
        this.orderby = orderby;
        return this;
    }

    /// <summary>
    /// Orders the results by a property in ascending order using a LINQ expression.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to order by.</typeparam>
    /// <param name="expression">Expression selecting the property to order by.</param>
    /// <returns>An OrderByBuilder for chaining ThenBy operations.</returns>
    public OrderByBuilder<T> Order<TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var visitor = new ODataOrderByVisitor();
        var propertyName = visitor.ToOrderBy(expression);

        this.orderby = propertyName;

        return new OrderByBuilder<T>(this);
    }

    /// <summary>
    /// Orders the results by a property in descending order using a LINQ expression.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to order by.</typeparam>
    /// <param name="expression">Expression selecting the property to order by descending.</param>
    /// <returns>An OrderByBuilder for chaining ThenBy operations.</returns>
    public OrderByBuilder<T> OrderDescending<TProperty>(Expression<Func<T, TProperty>> expression)
    {
        var visitor = new ODataOrderByVisitor();
        var propertyName = visitor.ToOrderBy(expression);

        this.orderby = $"{propertyName} desc";

        return new OrderByBuilder<T>(this);
    }

    /// <summary>
    /// Internal method to append orderby clauses from OrderByBuilder.
    /// </summary>
    internal void AppendOrderBy(string propertyName, bool descending)
    {
        var visitor = new ODataOrderByVisitor();
        this.orderby = visitor.AppendOrderBy(this.orderby ?? "", propertyName, descending);
    }

    public async Task<List<T>?> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var result = await odataClient.QueryEntitySetAsync<T>(EntityPath, select, expand, filter, count, top, skip, orderby, cancellationToken);
        return result?.Data;
    }



    public QueryProjectionBuilder<T, TResult> Select<TResult>(Expression<Func<List<T>, TResult>> selector)
    {

        return new QueryProjectionBuilder<T, TResult>(this, selector);
    }
}
