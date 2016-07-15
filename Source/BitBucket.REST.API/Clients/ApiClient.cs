using BitBucket.REST.API.Models;
using RestSharp;

namespace BitBucket.REST.API.Clients
{
    public abstract class ApiClient
    {
        protected ApiClient(RestClient restClient, Connection connection)
        {
            RestClient = restClient;
            Connection = connection;
        }

        protected Connection Connection { get; private set; }

        protected RestClient RestClient { get; private set; }
    }
}