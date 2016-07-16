using System.Threading.Tasks;
using BitBucket.REST.API.Clients;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Wrappers;

namespace BitBucket.REST.API
{
    public class BitbucketClientInitializer
    {
        public BitbucketClientInitializer(Connection connection)
        {
            var client = new BitbucketRestClient(connection);
            this.userClient = new UserClient(client, connection);

            this.connection = connection;
        }

        public async Task<BitbucketClient> Initialize()
        {
            var response = await this.userClient.GetUser();
            // to handle email address 
            var connectionWithUsername = new Connection(new Credentials(response.Username, this.connection.Credentials.Password));
            return new BitbucketClient(connectionWithUsername);
        }

        private Connection connection;
        private UserClient userClient;

    }
}