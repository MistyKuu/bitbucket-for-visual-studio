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
using System.Collections.Generic;
using BitBucket.REST.API.QueryBuilders;

namespace BitBucket.REST.API.Clients.Enterprise
{
    public class EnterpriseRepositoriesClient : ApiClient, IRepositoriesClient
    {
        public EnterpriseRepositoriesClient(EnterpriseBitbucketRestClient restClient, Connection connection) : base(restClient, connection)
        {
        }

        public async Task<IEnumerable<Repository>> GetRepositories(string owner)
        {
            return await GetRepositories();
        }

        public async Task<IEnumerable<Repository>> GetRepositories()
        {
            var url = EnterpriseApiUrls.Repositories();
            var repos = await RestClient.GetAllPages<EnterpriseRepository>(url);
            return repos.MapTo<List<Repository>>();
        }

        public async Task<IEnumerable<Repository>> GetRecentRepositories()
        {
            var url = EnterpriseApiUrls.RepositoriesRecent();
            var repos = await RestClient.GetAllPages<EnterpriseRepository>(url);
            return repos.MapTo<List<Repository>>();
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

        public async Task<Commit> GetCommitById(string repoName, string owner, string id)
        {
            var url = EnterpriseApiUrls.Commit(owner, repoName, id);
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync<EnterpriseCommit>(request);
            return response.Data.MapTo<Commit>();
        }

        public async Task<IEnumerable<Commit>> GetCommitsRange(string repoName, string owner, Branch fromBranch, Branch toBranch)
        {
            var url = EnterpriseApiUrls.Commits(owner, repoName);

            var queryString = new QueryString()
            {
                {"until", fromBranch.Target.Hash   },
                {"since",toBranch.Target.Hash },
            };

            var response = await RestClient.GetAllPages<EnterpriseCommit>(url, query: queryString);
            return response.MapTo<List<Commit>>();
        }

        public async Task<IEnumerable<Branch>> GetBranches(string repoName, string owner)
        {
            var url = EnterpriseApiUrls.Branches(owner, repoName);
            var branches = await RestClient.GetAllPages<EnterpriseBranch>(url);

            var result = branches.MapTo<List<Branch>>();

            return result;
        }
    }
}