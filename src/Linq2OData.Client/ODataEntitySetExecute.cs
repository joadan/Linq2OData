using System.Linq.Expressions;


namespace Linq2OData.Client;


public class ODataEntityExecute<T, TResult>(ODataEntityQuery<T> oDataQuery, Expression<Func<T, TResult>>? selector)
{

    public T? BaseResult { get; set; }

    public async Task<T?> ExecuteBaseAsync(CancellationToken cancellationToken = default)
    {
        //var exexutor = new QueryExecutor<T>(oDataQuery);
        //BaseResult = await exexutor.ExecuteRequestAsync(cancellationToken);
        //return BaseResult;

        return default;
    }

    public async Task<TResult?> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await ExecuteBaseAsync(cancellationToken);

        if (BaseResult == null)
        {
            return default;
        }

        return default;

        //if (selector == null) { return (TResult?)(object)BaseResult; }

        //return selector.Compile().Invoke(BaseResult);

    }

}



public class ODataEntitySetExecute<T, TResult>(ODataEntitySetQuery<T> oDataQuery, Expression<Func<List<T>, TResult>>? selector)
{

    public List<T>? BaseResult { get; set; }

    public async Task<List<T>?> ExecuteBaseAsync(CancellationToken cancellationToken = default)
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

        if (selector == null) { return (TResult?)(object)BaseResult; }

        return selector.Compile().Invoke(BaseResult);

    }

}
