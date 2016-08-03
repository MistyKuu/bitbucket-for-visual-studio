using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using BitBucket.REST.API.Exceptions;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Wrappers;
using RestSharp;

namespace BitBucket.REST.API.Clients
{
    public class RepositoriesClient : ApiClient
    {
        public RepositoriesClient(BitbucketRestClient restClient, Connection connection) : base(restClient, connection)
        {
        }

        public async Task<IteratorBasedPage<Repository>> GetRepositories()
        {
            var url = ApiUrls.Repositories(Connection.Credentials.Login);
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync<IteratorBasedPage<Repository>>(request);
            return response.Data;
        }

        public async Task<IteratorBasedPage<Repository>> GetRepositories(string owner)
        {
            var url = ApiUrls.Repositories(owner);
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync<IteratorBasedPage<Repository>>(request);
            return response.Data;
        }

        public async Task<IteratorBasedPage<Branch>> GetBranches(string repoName)
        {
            var url = ApiUrls.Branches(Connection.Credentials.Login, repoName);
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync<IteratorBasedPage<Branch>>(request);
            return response.Data;
        }

        public async Task<Repository> CreateRepository(Repository repository)
        {
            var url = ApiUrls.CreateRepository(Connection.Credentials.Login, repository.Name.ToLower(CultureInfo.InvariantCulture));
            var request = new BitbucketRestRequest(url, Method.POST);
            request.AddParameter("application/json; charset=utf-8", request.JsonSerializer.Serialize(repository), ParameterType.RequestBody);
            var response = await RestClient.ExecuteTaskAsync<Repository>(request);
            return response.Data;
        }
    }
}