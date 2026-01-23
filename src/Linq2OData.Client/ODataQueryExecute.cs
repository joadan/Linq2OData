using System.Linq.Expressions;


namespace Linq2OData.Client;



public class ODataQueryExecute<T, TResult>(ODataQuery<T> oDataQuery, Expression<Func<T, TResult>>? selector)
{

    public T? BaseResult { get; set; }

    public async Task<T?> ExecuteBaseAsync(CancellationToken cancellationToken = default)
    {
        var exexutor = new QueryExecutor<T>(oDataQuery);
        BaseResult = await exexutor.ExecuteRequestAsync(cancellationToken);
        return BaseResult;
    }

    public async Task<TResult?> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await ExecuteBaseAsync(cancellationToken);

        if (BaseResult == null)
        {
            return default;
        }

        if (selector == null) {  return  (TResult?)(object)BaseResult; }

       return selector.Compile().Invoke(BaseResult);
     
    }

}
