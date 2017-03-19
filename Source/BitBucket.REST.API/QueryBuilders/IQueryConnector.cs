namespace BitBucket.REST.API.QueryBuilders
{
    public interface IQueryConnector : IQueryParam
    {
        string Build();
        IQueryParam And();
        IQueryParam Or();
    }
}