using System;
using System.Linq;
using System.Threading.Tasks;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Mappings;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.Wrappers;
using RestSharp;

namespace BitBucket.REST.API.Clients.Enterprise
{
    public class EnterpriseRepositoriesClient : ApiClient, IRepositoriesClient
    {
        public EnterpriseRepositoriesClient(EnterpriseBitbucketRestClient restClient, Connection connection) : base(restClient, connection)
        {
        }

        public async Task<IteratorBasedPage<Repository>> GetRepositories(string owner)
        {
            return await GetRepositories();
        }
        
        public async Task<IteratorBasedPage<Repository>> GetRepositories()
        {
            var url = EnterpriseApiUrls.Repositories();
            var repos = await RestClient.GetAllPages<EnterpriseRepository>(url);
            return repos.MapTo<IteratorBasedPage<Repository>>();
        }

        public async Task<IteratorBasedPage<Repository>> GetRecentRepositories()
        {
            var url = EnterpriseApiUrls.RepositoriesRecent();
            var repos = await RestClient.GetAllPages<EnterpriseRepository>(url);
            return repos.MapTo<IteratorBasedPage<Repository>>();
        }


        public async Task<Repository> CreateRepository(Repository repository)
        {
            var url = EnterpriseApiUrls.CreateRepositories(Connection.Credentials.Login);
            var request = new BitbucketRestRequest(url, Method.POST);
            var enterpriseRepo = new EnterpriseRepository()
            {
                Name = repository.Name,
                IsPublic = !repository.IsPrivate
            };

            request.AddParameter("application/json; charset=utf-8", request.JsonSerializer.Serialize(enterpriseRepo), ParameterType.RequestBody);
            var response = await RestClient.ExecuteTaskAsync<EnterpriseRepository>(request);

            return response.Data.MapTo<Repository>();
        }

        public async Task<IteratorBasedPage<Branch>> GetBranches(string repoName)
        {
            return await GetBranches(Connection.Credentials.Login, repoName);
        }

        public async Task<IteratorBasedPage<Branch>> GetBranches(string owner, string repoName)
        {
            var url = EnterpriseApiUrls.Branches(owner, repoName);
            var branches = await RestClient.GetAllPages<EnterpriseBranch>(url);

            var result = branches.MapTo<IteratorBasedPage<Branch>>();

            return result;
        }
    }
}