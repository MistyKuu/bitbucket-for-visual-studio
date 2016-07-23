using System;
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

        public async Task<Repository> CreateRepository(Repository repository)
        {
            var url = ApiUrls.CreateRepository(Connection.Credentials.Login, repository.Name);
            var request = new BitbucketRestRequest(url, Method.POST);
            request.AddParameter("application/json; charset=utf-8", request.JsonSerializer.Serialize(repository), ParameterType.RequestBody);
            var response = await RestClient.ExecuteTaskAsync<Repository>(request);
            return response.Data;
        }
    }
}