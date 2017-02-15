using System;
using System.Security.Policy;
using System.Threading.Tasks;
using BitBucket.REST.API.Clients;
using BitBucket.REST.API.Clients.Enterprise;
using BitBucket.REST.API.Clients.Standard;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.Wrappers;

namespace BitBucket.REST.API
{
    public class BitbucketClientFactory : IBitbucketClientFactory
    {
        public Task<IBitbucketClient> CreateEnterpriseBitBucketClient(Uri host, Credentials cred)
        {
            var apiConnection = new Connection(host, new Uri($"{host}rest/api/1.0/"), cred);
            IBitbucketClient client = new EnterpriseBitbucketClient(apiConnection);
            return Task.FromResult(client);
        }

        public async Task<IBitbucketClient> CreateStandardBitBucketClient(Uri host, Credentials cred)
        {
            var apiConnection = new Connection(host, new Uri($"{host.Scheme}://api.{host.Host}/2.0/"), cred);
            var client = new BitbucketRestClient(apiConnection);
            var userClient = new UserClient(client, apiConnection);
            var response = await userClient.GetUser();
            var credentials = new Credentials(response.Username, apiConnection.Credentials.Password);

            apiConnection = new Connection(host, new Uri($"{host.Scheme}://api.{host.Host}/2.0/"), credentials);

            var internalApiConnection = new Connection(host, new Uri($"{host.Scheme}://{host.Host}/!api/internal/"), credentials);

            return new BitbucketClient(apiConnection, internalApiConnection);
        }
    }
}