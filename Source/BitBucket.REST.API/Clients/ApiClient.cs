using System.Threading.Tasks;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.QueryBuilders;
using BitBucket.REST.API.Wrappers;
using RestSharp;

namespace BitBucket.REST.API.Clients
{
    public abstract class ApiClient
    {
        protected ApiClient(BitbucketRestClientBase restClient, Connection connection)
        {
            RestClient = restClient;
            Connection = connection;
        }

        protected ApiClient(BitbucketRestClientBase restClient, BitbucketRestClient internalRestClient,  Connection connection)
        {
            RestClient = restClient;
            Connection = connection;
            InternalRestClient = internalRestClient;
        }


        protected Connection Connection { get; private set; }

        protected BitbucketRestClientBase RestClient { get; private set; }
        protected BitbucketRestClientBase InternalRestClient { get; private set; }
    }
}