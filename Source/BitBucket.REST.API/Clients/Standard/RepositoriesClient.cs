using System;
using System.Globalization;
using System.Threading.Tasks;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.Wrappers;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using BitBucket.REST.API.Mappings;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.QueryBuilders;

namespace BitBucket.REST.API.Clients.Standard
{
    public class RepositoriesClient : ApiClient, IRepositoriesClient
    {

        public RepositoriesClient(IBitbucketRestClient restClient,
            Connection connection) : base(restClient, connection)
        {
        }

        public Task<IEnumerable<Repository>> GetUserRepositories()
        {
            return GetUserRepositoriesV2(Connection.Credentials.Login);
        }

        public async Task<IEnumerable<Repository>> GetUserRepositoriesV2(string username)
        {
            var url = ApiUrls.Repositories();
            var queryStr = new QueryString { { "role", "member" } };

            return await RestClient.GetAllPages<Repository>(url, limit: 50, queryStr);
        }

        public async Task<IEnumerable<Branch>> GetBranches(string repoName, string owner)
        {
            var url = ApiUrls.Branches(owner, repoName);
            var branches = (await RestClient.GetAllPages<Branch>(url)).ToList();
            return branches;
            try
            {//todo no endpoint?
                var defaultBranchResponse = await DefaultBranch(repoName, owner);
                var defaultBranch = branches.FirstOrDefault(x => x.Name == defaultBranchResponse?.Name);
                if (defaultBranch != null)
                    defaultBranch.IsDefault = true;
            }
            catch (Exception)
            {
                // no default branch
            }

            return branches;
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

        public async Task<Repository> CreateRepository(Repository repository, bool isTeam)
        {
            var login = isTeam ? repository.Owner.Username : Connection.Credentials.Login;
            var url = ApiUrls.Repository(login, repository.Name);
            var request = new BitbucketRestRequest(url, Method.POST);
            request.AddParameter("application/json; charset=utf-8", request.JsonSerializer.Serialize(repository), ParameterType.RequestBody);
            var response = await RestClient.ExecuteTaskAsync<Repository>(request);
            return response.Data;
        }

        private async Task<DefaultBranch> DefaultBranch(string repoName, string owner)
        {
            var url = ApiUrls.DefaultBranch(owner, repoName);
            var request = new BitbucketRestRequest(url, Method.GET);
            throw new NotSupportedException("Deprecated");
            //     var response = await _versionOneClient.ExecuteTaskAsync<DefaultBranch>(request);
            //  return response.Data;
        }
    }
}