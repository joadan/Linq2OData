using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

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

    //public async Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default)
    //{
        
    //    return await ExecuteBaseAsync(cancellationToken)

    //    return default;
    //    // return ConvertResult(await ExecuteBaseAsync(cancellationToken));
    //}
}
