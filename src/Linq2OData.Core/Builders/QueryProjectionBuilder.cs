using Linq2OData.Core.Expressions;
using Linq2OData.Core.ODataResponse;
using System.Linq.Expressions;

namespace Linq2OData.Core.Builders;



public class QueryProjectionBuilder<T, TResult>(QueryBuilder<T> queryBuilder, Expression<Func<List<T>, TResult>> selector) where T : IODataEntitySet, new()
{
    public ODataResponse<List<T>>? BaseResult { get; set; }

    public async Task<ODataResponse<List<T>>?> ExecuteBaseAsync(CancellationToken cancellationToken = default)
    {
        SetProjection();
        BaseResult = await queryBuilder.ODataClient.QueryEntitySetAsync<T>(queryBuilder.EntityPath, queryBuilder.select, queryBuilder.expand, queryBuilder.filter, queryBuilder.count, queryBuilder.top, queryBuilder.skip, queryBuilder.orderby, cancellationToken);
        return BaseResult;
    }


    private void SetProjection()
    {
        var visitor = new QueryNodeVisitor();
        var node = visitor.Parse(selector);
        var projected = node.GetSelectExpand(queryBuilder.ODataClient.ODataVersion);

        queryBuilder.select = projected.select;
        queryBuilder.expand = projected.expand;
    }

    public async Task<TResult?> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await ExecuteBaseAsync(cancellationToken);
        if (BaseResult?.Data == null)
        {
            return default;
        }

        return selector.Compile().Invoke(BaseResult.Data);

    }

}
