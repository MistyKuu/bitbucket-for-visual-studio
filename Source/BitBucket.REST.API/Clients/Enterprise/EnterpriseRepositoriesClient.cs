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

        public async Task<IteratorBasedPage<Repository>> GetRepositories()
        {
            return await GetRepositories(Connection.Credentials.Login);
        }

        public async Task<IteratorBasedPage<Repository>> GetRepositories(string owner)
        {
            var url = EnterpriseApiUrls.Repositories(owner);
            var repos = await RestClient.GetAllPages<EnterpriseRepository>(url);
            return repos.MapTo<IteratorBasedPage<Repository>>();
        }

        public async Task<Repository> CreateRepository(Repository repository)
        {
            var url = EnterpriseApiUrls.Repositories(Connection.Credentials.Login);
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

            foreach (var branch in result.Values)
            {
                var commitsUrl = EnterpriseApiUrls.Commits(owner, repoName, branch.Target.Hash);//todo find a better way to do it
                var request = new BitbucketRestRequest(commitsUrl, Method.GET);
                branch.Target = (await RestClient.ExecuteTaskAsync<EnterpriseCommit>(request)).Data.MapTo<Commit>();
            }


            return result;
        }
    }
}