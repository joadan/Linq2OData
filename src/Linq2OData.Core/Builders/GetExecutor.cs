using Linq2OData.Core.Metadata;
using Linq2OData.Core.ODataResponse;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Linq2OData.Core.Builders
{
    public class GetExecutor<T, TResult>(GetBuilder<T> getBuilder, Expression<Func<T, TResult>>? selector) where T : IODataEntitySet, new()
    {
        public ODataResponse<T>? BaseResult { get; set; }

        public async Task<ODataResponse<T>?> ExecuteBaseAsync(CancellationToken cancellationToken = default)
        {
            BaseResult = await getBuilder.ODataClient.QueryEntityAsync<T>(getBuilder.EntityPath, getBuilder.KeyExpression, getBuilder.expand);
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
}
