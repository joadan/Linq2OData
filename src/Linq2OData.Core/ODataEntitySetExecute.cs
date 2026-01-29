using Linq2OData.Core.ODataResponse;
using System.Linq.Expressions;


namespace Linq2OData.Core;


public class ODataEntityExecute<T, TResult>(ODataEntityQuery<T> oDataQuery, Expression<Func<T, TResult>>? selector)
{

    public ODataResponse<T>? BaseResult { get; set; }

    public async Task<ODataResponse<T>?> ExecuteBaseAsync(CancellationToken cancellationToken = default)
    {
        BaseResult = await oDataQuery.ODataClient.QueryEntityAsync<T>(oDataQuery.EntitySetName, oDataQuery.KeyString, oDataQuery.ExpandValue);
        return BaseResult;

    }

    public async Task<TResult?> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await ExecuteBaseAsync(cancellationToken);

        if (BaseResult == null)
        {
            return default;
        }

        if (BaseResult.Data == null)
        {
            return default;
        }

        if (selector == null) { return (TResult?)(object?)BaseResult.Data; }

        return selector.Compile().Invoke(BaseResult.Data!);

    }

}



public class ODataEntitySetExecute<T, TResult>(ODataEntitySetQuery<T> oDataQuery, Expression<Func<List<T>, TResult>>? selector)
{

    public ODataResponse<List<T>>? BaseResult { get; set; }

    public async Task<ODataResponse<List<T>>?> ExecuteBaseAsync(CancellationToken cancellationToken = default)
    {
        BaseResult = await oDataQuery.ODataClient.QueryEntitySetAsync<T>(oDataQuery.EntitySetName, oDataQuery.ExpandValue, oDataQuery.FilterValue, oDataQuery.CountValue, oDataQuery.TopValue, oDataQuery.SkipValue, cancellationToken );
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
