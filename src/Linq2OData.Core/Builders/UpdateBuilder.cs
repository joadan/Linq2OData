using Linq2OData.Core.ODataResponse;

namespace Linq2OData.Core.Builders;




public class UpdateBuilder<T> where T : IODataEntitySet, new()
{

    private ODataClient odataClient;
    private string entityPath;
    private string keyExpression;

    public UpdateBuilder(ODataClient odataClient, Action<T> keySetter)
    {
        this.odataClient = odataClient;
        entityPath = BuilderHelper.GetEntityPath<T>();
        keyExpression = BuilderHelper.GetEntityKeys<T>(keySetter);
    }

   
    public async Task<bool> ExecuteAsync(ODataInputBase input)
    {
      return  await odataClient.UpdateEntityAsync(entityPath, keyExpression, input);
    }

    /// <summary>
    /// Executes the update request and returns the full <see cref="ODataResponse{T}"/> including
    /// HTTP status code and response headers.
    /// </summary>
    public async Task<ODataResponse<bool>> ExecuteResponseAsync(ODataInputBase input)
    {
        return await odataClient.UpdateEntityWithResponseAsync(entityPath, keyExpression, input);
    }




}
