using System.Threading.Tasks;
using AutoMapper;
using BitBucket.REST.API.Clients;
using BitBucket.REST.API.Clients.Enterprise;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Mappings;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.Wrappers;
using RestSharp;

namespace BitBucket.REST.API
{
    public class EnterpriseBitbucketClient : IBitbucketClient
    {
        private readonly EnterpriseBitbucketRestClient _client;
        public ITeamsClient TeamsClient { get; }
        public IRepositoriesClient RepositoriesClient { get; }
        public IUserClient UserClient { get; }
        public IPullRequestsClient PullRequestsClient { get; }
        public Connection ApiConnection { get; }
        public BitBucketType BitBucketType { get; } = BitBucketType.Enterprise;
        public EnterpriseBitbucketClient(Connection apiConnection,IProxyResolver proxyResolver)
        {
            ApiConnection = apiConnection;
            _client = new EnterpriseBitbucketRestClient(apiConnection) { ProxyResolver = proxyResolver };
            RepositoriesClient = new EnterpriseRepositoriesClient(_client, ApiConnection);
            UserClient = new EnterpriseUserClient(_client, ApiConnection);
            PullRequestsClient = new EnterprisePullRequestsClient(_client, ApiConnection);
            TeamsClient = new EnterpriseTeamsClient(_client, ApiConnection);
        }
    }
}