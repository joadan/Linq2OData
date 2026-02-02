namespace Linq2OData.Core.Builders;

public class KeyBuilder<T>(ODataClient oDataClient, string entityPath, string keyExpression) where T : IODataEntitySet, new()
{


    public async Task<bool> DeleteAsync()
    {
        return await oDataClient.DeleteEntityAsync(entityPath, keyExpression);
    }

    public GetBuilder<T> Get()
    {
        return new GetBuilder<T>(oDataClient, entityPath, keyExpression);
    }



}
