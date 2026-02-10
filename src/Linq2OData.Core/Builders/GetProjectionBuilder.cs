using Linq2OData.Core.Expressions;
using Linq2OData.Core.ODataResponse;
using System.Linq.Expressions;

namespace Linq2OData.Core.Builders;



public class GetProjectionBuilder<T, TResult>(GetBuilder<T> getBuilder, Expression<Func<T, TResult>> selector) where T : IODataEntitySet, new()
{
    public ODataResponse<T>? BaseResult { get; set; }

    public async Task<ODataResponse<T>?> ExecuteBaseAsync(CancellationToken cancellationToken = default)
    {
        SetProjection();
        BaseResult = await getBuilder.ODataClient.QueryEntityAsync<T>(getBuilder.EntityPath, getBuilder.KeyExpression, getBuilder.select, getBuilder.expand, cancellationToken);
        return BaseResult;
    }


    private void SetProjection()
    {
        var visitor = new SelectExpressionVisitor();
        var node = visitor.Parse(selector);
        var projected = node.GetSelectExpand(getBuilder.ODataClient.ODataVersion);

        getBuilder.select = projected.select;
        getBuilder.expand = projected.expand;
    }

    public async Task<TResult?> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await ExecuteBaseAsync(cancellationToken);
        if (BaseResult == null)
        {
            return default;
        }

        return selector.Compile().Invoke(BaseResult.Data);

    }

}
