namespace BitBucket.REST.API.QueryBuilders
{
    public interface IQueryConnector
    {
        IQueryParam And();
        IQueryParam Or();
        string Build();
    }
}