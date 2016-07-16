using BitBucket.REST.API.Models;
using BitBucket.REST.API.Wrappers;
using RestSharp;

namespace BitBucket.REST.API.Clients
{
    public abstract class ApiClient
    {
        protected ApiClient(BitbucketRestClient restClient, Connection connection)
        {
            RestClient = restClient;
            Connection = connection;
        }

        protected Connection Connection { get; private set; }

        protected BitbucketRestClient RestClient { get; private set; }
    }
}