using System.Globalization;
using System.Threading.Tasks;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.Wrappers;
using RestSharp;
using System.Collections.Generic;
using BitBucket.REST.API.Mappings;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.QueryBuilders;

namespace BitBucket.REST.API.Clients.Standard
{
    public class RepositoriesClient : ApiClient, IRepositoriesClient
    {
        public RepositoriesClient(BitbucketRestClient restClient, Connection connection) : base(restClient, connection)
        {
        }

        public async Task<IEnumerable<Repository>> GetRepositories()
        {
            return await GetRepositories(Connection.Credentials.Login);
        }

        public async Task<IEnumerable<Repository>> GetRepositories(string owner)
        {
            var url = ApiUrls.Repositories(owner);
            return await RestClient.GetAllPages<Repository>(url);
        }

        public async Task<IEnumerable<Branch>> GetBranches(string repoName)
        {
            return await GetBranches(Connection.Credentials.Login, repoName);
        }

        public async Task<IEnumerable<Branch>> GetBranches(string owner, string repoName)
        {
            var url = ApiUrls.Branches(owner, repoName);
            return await RestClient.GetAllPages<Branch>(url);
        }



        public async Task<Commit> GetCommitById(string repoName, string owner, string id)
        {
            var url = ApiUrls.Commit(owner, repoName, id);
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync<Commit>(request);
            return response.Data;
        }

        public async Task<IEnumerable<Commit>> GetCommitsRange(string repoName, string owner, Branch fromBranch, Branch toBranch)
        {
            var url = ApiUrls.Commits(owner, repoName, fromBranch.Name);

            var queryString = new QueryString()
            {
                {"exclude",toBranch.Name },
            };

            var response = await RestClient.GetAllPages<Commit>(url, query: queryString);
            return response;
        }

        public async Task<Repository> CreateRepository(Repository repository)
        {
            var url = ApiUrls.Repository(Connection.Credentials.Login, repository.Name.Replace(' ', '-').ToLower(CultureInfo.InvariantCulture));
            var request = new BitbucketRestRequest(url, Method.POST);
            request.AddParameter("application/json; charset=utf-8", request.JsonSerializer.Serialize(repository), ParameterType.RequestBody);
            var response = await RestClient.ExecuteTaskAsync<Repository>(request);
            return response.Data;
        }
    }
}