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
        public BitbucketClient(Connection connection, Connection internaConnection)
        {
            Connection = connection;
            Client = new BitbucketRestClient(connection);
            InternalClient = new BitbucketRestClient(internaConnection);
            RepositoriesClient = new RepositoriesClient(Client, Connection);
            UserClient = new UserClient(Client, Connection);
            PullRequestsClient = new PullRequestsClient(Client, InternalClient, Connection);
            TeamsClient = new TeamsClient(Client, Connection);
        }

        public void Initialize()
        {
            
        }


        public TeamsClient TeamsClient { get; private set; }
        public RepositoriesClient RepositoriesClient { get; private set; }
        public UserClient UserClient { get; private set; }
        public Connection Connection { get; private set; }
        public PullRequestsClient PullRequestsClient { get; private set; }
        
        private BitbucketRestClient Client { get; set; }
        private BitbucketRestClient InternalClient { get; set; }

    }
}