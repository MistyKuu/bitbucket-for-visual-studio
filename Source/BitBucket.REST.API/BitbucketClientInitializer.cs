using System;
using System.Threading.Tasks;
using BitBucket.REST.API.Clients;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Wrappers;

namespace BitBucket.REST.API
{
    public class BitbucketClientInitializer
    {
        public async Task<BitbucketClient> Initialize(Credentials cred)
        {
            var apiConnection = new Connection(new Uri("https://api.bitbucket.org/2.0/"), cred);

            var client = new BitbucketRestClient(apiConnection);
            var userClient = new UserClient(client, apiConnection);

            var response = await userClient.GetUser();
            var credentials = new Credentials(response.Username, apiConnection.Credentials.Password);

            var internalApiConnection = new Connection(new Uri("https://bitbucket.org/!api/internal/"), credentials);

            return new BitbucketClient(apiConnection, internalApiConnection);
        }

    }
}