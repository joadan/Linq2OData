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




}
