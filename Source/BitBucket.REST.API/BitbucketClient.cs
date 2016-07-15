using BitBucket.REST.API.Authentication;
using BitBucket.REST.API.Clients;
using BitBucket.REST.API.Models;
using RestSharp;

namespace BitBucket.REST.API
{
    public class BitbucketClient
    {
        public BitbucketClient(Connection connection)
        {
            Connection = connection;
            Client = new RestClient(connection.BitbucketUrl);
            // todo: make it a little simpler
            var auth = new Authenticator(connection.Credentials);
            Client.Authenticator = auth.Authenticators[connection.Credentials.AuthenticationType];
            RepositoriesClient = new RepositoriesClient(Client, Connection);

        }

        public RepositoriesClient RepositoriesClient { get; private set; }
        public Connection Connection { get; private set; }
        
        private RestClient Client { get; set; }
    }
}