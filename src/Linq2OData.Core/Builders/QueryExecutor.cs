using Linq2OData.Core.ODataResponse;
using System.Linq.Expressions;

namespace Linq2OData.Core.Builders;


public class QueryExecutor<T, TResult>(QueryBuilder<T> queryBuilder, Expression<Func<List<T>, TResult>>? selector) where T : IODataEntitySet, new() 
{
    public ODataResponse<List<T>>? BaseResult { get; set; }

    public async Task<ODataResponse<List<T>>?> ExecuteBaseAsync(CancellationToken cancellationToken = default)
    {
        BaseResult = await queryBuilder.ODataClient.QueryEntitySetAsync<T>(queryBuilder.EntityPath, queryBuilder.select, queryBuilder.expand, queryBuilder.filter, queryBuilder.count, queryBuilder.top, queryBuilder.skip, queryBuilder.orderby, cancellationToken);
        return BaseResult;
    }

    public async Task<TResult?> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await ExecuteBaseAsync(cancellationToken);

        if (BaseResult?.Data == null)
        {
            return default;
        }

        if (selector == null) { return (TResult?)(object)BaseResult.Data; }

        return selector.Compile().Invoke(BaseResult.Data);

    }

}
