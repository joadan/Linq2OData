using Linq2OData.Core.ODataResponse;

namespace Linq2OData.Core.Builders;




public class DeleteBuilder<T> where T : IODataEntitySet, new()
{

    private ODataClient odataClient;
    private string entityPath;
    private string keyExpression;

    public DeleteBuilder(ODataClient odataClient, Action<T> keySetter)
    {
        this.odataClient = odataClient;
        entityPath = BuilderHelper.GetEntityPath<T>();
        keyExpression = BuilderHelper.GetEntityKeys<T>(keySetter);
    }

   
    public async Task<bool> ExecuteAsync()
    {
      return  await odataClient.DeleteEntityAsync(entityPath, keyExpression);
    }

    /// <summary>
    /// Executes the delete request and returns the full <see cref="ODataResponse{T}"/> including
    /// HTTP status code and response headers.
    /// </summary>
    public async Task<ODataResponse<bool>> ExecuteResponseAsync()
    {
        return await odataClient.DeleteEntityWithResponseAsync(entityPath, keyExpression);
    }




}
