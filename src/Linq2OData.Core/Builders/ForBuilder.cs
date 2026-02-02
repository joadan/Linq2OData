using System.Reflection;

namespace Linq2OData.Core.Builders;


public class ForBuilder<T>
  where T : IODataEntitySet, new()
{
    private ODataClient odataClient;
    private string entityPath;

    public ForBuilder(ODataClient odataClient)
    {
        this.odataClient = odataClient;

        var attrib = typeof(T).GetCustomAttribute<ODataEntitySetAttribute>();
        if (attrib == null)
        {
            throw new InvalidOperationException($"The type {typeof(T).FullName} is missing the ODataEntitySetAttribute.");
        }

        this.entityPath = attrib.EntityPath;
    }

    public async Task<T> CreateAsync(ODataInputBase input)
    {
        return await odataClient.CreateEntityAsync<T>(entityPath, input);
    }

    public KeyBuilder<T> Key<TKey>(Action<TKey> keySetter) where TKey : IODataEntitySet, new()
    {
        var keyEntity = new TKey();
        keySetter(keyEntity);

        return new KeyBuilder<T>(odataClient, entityPath, keyEntity.__Keys);
    }

    public KeyBuilder<T> Key(Action<T> keySetter)
    {
        var entity = new T();
        keySetter(entity);
        return new KeyBuilder<T>(odataClient, entityPath, entity.__Keys);
    }


    public QueryBuilder<T> Query()
    {
        return new QueryBuilder<T>(odataClient, entityPath);
    }
}
