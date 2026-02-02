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




}
