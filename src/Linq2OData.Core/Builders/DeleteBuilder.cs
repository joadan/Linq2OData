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




}
