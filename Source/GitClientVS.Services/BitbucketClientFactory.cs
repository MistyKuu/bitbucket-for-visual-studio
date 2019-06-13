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
using log4net;
using BitBucket.REST.API;
using GitClientVS.Contracts.Interfaces;
using System.ComponentModel.Composition;

namespace GitClientVS.Services
{
    [Export(typeof(IBitbucketClientFactory))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class BitbucketClientFactory : IBitbucketClientFactory
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IProxyResolver _proxyResolver;

        [ImportingConstructor]
        public BitbucketClientFactory(IProxyResolver proxyResolver)
        {
            _proxyResolver = proxyResolver;
        }

        public async Task<IBitbucketClient> CreateEnterpriseBitBucketClient(Uri host, Credentials cred)
        {
            Logger.Info($"Calling CreateEnterpriseBitBucketClient. Host: {host}");

            var apiConnection = new Connection(host, new Uri(host, "rest/api/1.0"), cred);
            EnterpriseBitbucketClient client = new EnterpriseBitbucketClient(apiConnection, _proxyResolver);
            await ((EnterpriseRepositoriesClient)client.RepositoriesClient).GetRecentRepositories();//will throw exception if not authenticated
            return client;
        }

        public async Task<IBitbucketClient> CreateStandardBitBucketClient(Credentials cred)
        {
            Logger.Info($"Calling CreateStandardBitBucketClient.");

            var host = new Uri("https://bitbucket.org");
            var apiConnection = new Connection(host, new Uri($"{host.Scheme}://api.{host.Host}/2.0/"), cred);
            var client = new BitbucketRestClient(apiConnection) { ProxyResolver = _proxyResolver };
            var userClient = new UserClient(client, apiConnection);
            var response = await userClient.GetUser();
            var credentials = new Credentials(response.Username, apiConnection.Credentials.Password, response.Uuid);

            apiConnection = new Connection(host, new Uri($"{host.Scheme}://api.{host.Host}/2.0/"), credentials);

            return new BitbucketClient(apiConnection,  _proxyResolver);
        }
    }
}