using AutoMapper;
using BitBucket.REST.API.Clients;
using BitBucket.REST.API.Clients.Enterprise;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Mappings;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.Wrappers;

namespace BitBucket.REST.API
{
    public class EnterpriseBitbucketClient : IBitbucketClient
    {
        public ITeamsClient TeamsClient { get; }
        public IRepositoriesClient RepositoriesClient { get; }
        public IUserClient UserClient { get; }
        public IPullRequestsClient PullRequestsClient { get; }
        public Connection ApiConnection { get; }
        
        public EnterpriseBitbucketClient(Connection apiConnection)
        {
            ApiConnection = apiConnection;
            var client = new EnterpriseBitbucketRestClient(apiConnection);
            RepositoriesClient = new EnterpriseRepositoriesClient(client, ApiConnection);
            UserClient = new EnterpriseUserClient(client, ApiConnection);
            PullRequestsClient = new EnterprisePullRequestsClient(client, ApiConnection);
            TeamsClient = new EnterpriseTeamsClient(client, ApiConnection);
        }
    }
}