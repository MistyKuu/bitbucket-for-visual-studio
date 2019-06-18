using System.Threading.Tasks;
using BitBucket.REST.API.Authentication;
using BitBucket.REST.API.Clients;
using BitBucket.REST.API.Clients.Standard;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.Serializers;
using BitBucket.REST.API.Wrappers;
using RestSharp;

namespace BitBucket.REST.API
{
    public class BitbucketClient : IBitbucketClient
    {
        private readonly BitbucketRestClient _client;
        public ITeamsClient TeamsClient { get; }
        public IRepositoriesClient RepositoriesClient { get; }
        public IUserClient UserClient { get; }
        public IPullRequestsClient PullRequestsClient { get; }
        public Connection ApiConnection { get; }
        public BitBucketType BitBucketType { get; } = BitBucketType.Standard;

        public BitbucketClient(
            Connection apiConnection,
            IProxyResolver proxyResolver
            )
        {
            ApiConnection = apiConnection;
            _client = new BitbucketRestClient(apiConnection) { ProxyResolver = proxyResolver };

            RepositoriesClient = new RepositoriesClient(_client, ApiConnection);
            UserClient = new UserClient(_client, ApiConnection);
            PullRequestsClient = new PullRequestsClient(_client, ApiConnection);
            TeamsClient = new TeamsClient(_client, ApiConnection);
        }
    }
}