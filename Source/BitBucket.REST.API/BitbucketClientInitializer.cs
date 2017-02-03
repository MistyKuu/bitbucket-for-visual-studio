using System;
using System.Security.Policy;
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
            var uri = new Uri(cred.Host);

            var apiConnection = new Connection(new Uri($"{uri.Scheme}://api.{uri.Host}/2.0/"), cred);
            var client = new BitbucketRestClient(apiConnection);
            var userClient = new UserClient(client, apiConnection);

            var response = await userClient.GetUser();
            var credentials = new Credentials(response.Username, apiConnection.Credentials.Password) {Host = cred.Host};

            apiConnection = new Connection(new Uri($"{uri.Scheme}://api.{uri.Host}/2.0/"), credentials);

            var internalApiConnection = new Connection(new Uri($"{uri.Scheme}://{uri.Host}/!api/internal/"), credentials);

            return new BitbucketClient(apiConnection, internalApiConnection);
        }

    }
}