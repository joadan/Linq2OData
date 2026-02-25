namespace Linq2OData.Core.Builders;

public class SingletonBuilder<T> where T : new()
{
    private ODataClient odataClient;
    private string singletonPath;

    public SingletonBuilder(ODataClient odataClient, string singletonPath)
    {
        this.odataClient = odataClient;
        this.singletonPath = singletonPath;
    }

    internal string? select;
    internal string? expand;

    public SingletonBuilder<T> Expand(string? expand = null)
    {
        this.expand = expand;
        return this;
    }

    public SingletonBuilder<T> Select(string? select = null)
    {
        this.select = select;
        return this;
    }

    public async Task<T?> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var result = await odataClient.QueryEntityAsync<T>(singletonPath, string.Empty, select, expand, cancellationToken);
        if (result == null) { return default; }
        return result.Data;
    }
}
