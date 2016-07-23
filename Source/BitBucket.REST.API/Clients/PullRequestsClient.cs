using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Wrappers;
using RestSharp;

namespace BitBucket.REST.API.Clients
{
    public class PullRequestsClient : ApiClient
    {
        public PullRequestsClient(BitbucketRestClient restClient, Connection connection) : base(restClient, connection)
        {
            
        }


        public async Task<IteratorBasedPage<PullRequest>> GetPullRequests(Repository repository)
        {
            var url = ApiUrls.PullRequests(Connection.Credentials.Login, repository.Name);
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync<IteratorBasedPage<PullRequest>>(request);
            return response.Data;
        }

        public async Task<IteratorBasedPage<PullRequest>> GetPullRequests(Repository repository, PullRequestOptions option)
        {
            var url = ApiUrls.PullRequests(Connection.Credentials.Login, repository.Name);
            var request = new BitbucketRestRequest(url, Method.GET);
            request.AddQueryParameter("state", option.ToString());
            var response = await RestClient.ExecuteTaskAsync<IteratorBasedPage<PullRequest>>(request);
            return response.Data;
        }

        public async Task<PullRequest> GetPullRequest(Repository repository, long id)
        {
            var url = ApiUrls.PullRequest(Connection.Credentials.Login, repository.Name, id);
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync<PullRequest>(request);
            return response.Data;
        }

        public async Task<PullRequest> CreatePullRequest(PullRequest pullRequest, Repository repository)
        {
            var url = ApiUrls.PullRequests(Connection.Credentials.Login, repository.Name);
            var request = new BitbucketRestRequest(url, Method.POST);
            request.AddParameter("application/json; charset=utf-8", pullRequest, ParameterType.RequestBody);
            var response = await RestClient.ExecuteTaskAsync<PullRequest>(request);
            return response.Data;
        }


    }
}