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
        public ITeamsClient TeamsClient { get; }
        public IRepositoriesClient RepositoriesClient { get; }
        public IUserClient UserClient { get; }
        public IPullRequestsClient PullRequestsClient { get; }
        public Connection ApiConnection { get; }
        public BitBucketType BitBucketType { get; } = BitBucketType.Standard;

        public BitbucketClient(Connection apiConnection, Connection internalConnection)
        {
            ApiConnection = apiConnection;
            var client = new BitbucketRestClient(apiConnection);
            var internalClient = new BitbucketRestClient(internalConnection);
            RepositoriesClient = new RepositoriesClient(client, ApiConnection);
            UserClient = new UserClient(client, ApiConnection);
            PullRequestsClient = new PullRequestsClient(client, internalClient, ApiConnection);
            TeamsClient = new TeamsClient(client, ApiConnection);
        }
    }
}