using Linq2OData.Core.ODataResponse;

namespace Linq2OData.Core.Builders;




public class CreateBuilder<T> where T : IODataEntitySet, new()
{
    private ODataClient odataClient;
    private string entityPath;

    public CreateBuilder(ODataClient odataClient)
    {
        this.odataClient = odataClient;
        entityPath = BuilderHelper.GetEntityPath<T>();
      
    }

   
    public async Task<T> ExecuteAsync(ODataInputBase input)
    {
      return  await odataClient.CreateEntityAsync<T>(entityPath,  input);
    }

    /// <summary>
    /// Executes the create request and returns the full <see cref="ODataResponse{T}"/> including
    /// HTTP status code and response headers.
    /// </summary>
    public async Task<ODataResponse<T>> ExecuteResponseAsync(ODataInputBase input)
    {
        return await odataClient.CreateEntityWithResponseAsync<T>(entityPath, input);
    }




}
