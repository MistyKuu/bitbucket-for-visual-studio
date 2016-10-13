using BitBucket.REST.API.Authentication;
using BitBucket.REST.API.Clients;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Serializers;
using BitBucket.REST.API.Wrappers;
using RestSharp;

namespace BitBucket.REST.API
{
    public class BitbucketClient
    {
        public BitbucketClient(Connection apiConnection, Connection internalConnection)
        {
            ApiConnection = apiConnection;
            Client = new BitbucketRestClient(apiConnection);
            InternalClient = new BitbucketRestClient(internalConnection);
            RepositoriesClient = new RepositoriesClient(Client, ApiConnection);
            UserClient = new UserClient(Client, ApiConnection);
            PullRequestsClient = new PullRequestsClient(Client, InternalClient, ApiConnection);
            TeamsClient = new TeamsClient(Client, ApiConnection);
        }

        public void Initialize()
        {
            
        }


        public TeamsClient TeamsClient { get; private set; }
        public RepositoriesClient RepositoriesClient { get; private set; }
        public UserClient UserClient { get; private set; }
        public Connection ApiConnection { get; private set; }
        public PullRequestsClient PullRequestsClient { get; private set; }
        
        private BitbucketRestClient Client { get; set; }
        private BitbucketRestClient InternalClient { get; set; }

    }
}