using System.Globalization;
using System.Threading.Tasks;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.Wrappers;
using RestSharp;

namespace BitBucket.REST.API.Clients.Standard
{
    public class RepositoriesClient : ApiClient, IRepositoriesClient
    {
        public RepositoriesClient(BitbucketRestClient restClient, Connection connection) : base(restClient, connection)
        {
        }

        public async Task<IteratorBasedPage<Repository>> GetRepositories()
        {
            return await GetRepositories(Connection.Credentials.Login);
        }

        public async Task<IteratorBasedPage<Repository>> GetRepositories(string owner)
        {
            var url = ApiUrls.Repositories(owner);
            return await RestClient.GetAllPages<Repository>(url);
        }

        public async Task<IteratorBasedPage<Branch>> GetBranches(string repoName)
        {
            return await GetBranches(Connection.Credentials.Login, repoName);
        }

        public async Task<IteratorBasedPage<Branch>> GetBranches(string owner, string repoName)
        {
            var url = ApiUrls.Branches(owner, repoName);
            return await RestClient.GetAllPages<Branch>(url);
        }

        public async Task<Repository> CreateRepository(Repository repository)
        {
            var url = ApiUrls.CreateRepository(Connection.Credentials.Login, repository.Name.Replace(' ','-').ToLower(CultureInfo.InvariantCulture));
            var request = new BitbucketRestRequest(url, Method.POST);
            request.AddParameter("application/json; charset=utf-8", request.JsonSerializer.Serialize(repository), ParameterType.RequestBody);
            var response = await RestClient.ExecuteTaskAsync<Repository>(request);
            return response.Data;
        }
    }
}