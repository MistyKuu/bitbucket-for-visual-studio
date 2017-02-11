using AutoMapper;
using BitBucket.REST.API.Authentication;
using BitBucket.REST.API.Clients;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Mappings;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.Serializers;
using BitBucket.REST.API.Wrappers;
using RestSharp;

namespace BitBucket.REST.API
{
    public class BitbucketClient : IBitbucketClient
    {
        public ITeamsClient TeamsClient { get; }
        public IRepositoriesClient RepositoriesClient { get; }
        public IUserClient UserClient { get; }
        public IPullRequestsClient PullRequestsClient { get; }
        public Connection ApiConnection { get; }

        public BitbucketClient(Connection apiConnection, Connection internalConnection)
        {
            ApiConnection = apiConnection;
            var client = new BitbucketRestClient(apiConnection);
            var internalClient = new BitbucketRestClient(internalConnection);
            RepositoriesClient = new RepositoriesClient(client, ApiConnection);
            UserClient = new UserClient(client, ApiConnection);
            PullRequestsClient = new PullRequestsClient(client, internalClient, ApiConnection);
            TeamsClient = new TeamsClient(client, ApiConnection);
        }
    }

    public class EnterpriseBitbucketClient : IBitbucketClient
    {
        public ITeamsClient TeamsClient { get; }
        public IRepositoriesClient RepositoriesClient { get; }
        public IUserClient UserClient { get; }
        public IPullRequestsClient PullRequestsClient { get; }
        public Connection ApiConnection { get; }

        static EnterpriseBitbucketClient() //todo correct place?
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<EnterpriseToStandardMappingsProfile>();
            });
        }

        public EnterpriseBitbucketClient(Connection apiConnection, Connection internalConnection)
        {
            ApiConnection = apiConnection;
            var client = new EnterpriseBitbucketRestClient(apiConnection);
            var internalClient = new EnterpriseBitbucketRestClient(internalConnection);
            RepositoriesClient = new EnterpriseRepositoriesClient(client, ApiConnection);
            // UserClient = new UserClient(client, ApiConnection);
            //  PullRequestsClient = new PullRequestsClient(client, internalClient, ApiConnection);
            //  TeamsClient = new TeamsClient(client, ApiConnection);
        }
    }
}