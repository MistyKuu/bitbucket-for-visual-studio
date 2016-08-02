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
        public BitbucketClient(Connection connection)
        {
            Connection = connection;
            Client = new BitbucketRestClient(connection);
            RepositoriesClient = new RepositoriesClient(Client, Connection);
            UserClient = new UserClient(Client, Connection);
            PullRequestsClient = new PullRequestsClient(Client, Connection);
            TeamsClient = new TeamsClient(Client, Connection);
        }

        public void Initialize()
        {
            
        }

        public string GetHost()
        {
            return "bitbucket.org";
        }

        public TeamsClient TeamsClient { get; private set; }
        public RepositoriesClient RepositoriesClient { get; private set; }
        public UserClient UserClient { get; private set; }
        public Connection Connection { get; private set; }
        public PullRequestsClient PullRequestsClient { get; private set; }
        
        private BitbucketRestClient Client { get; set; }

    }
}