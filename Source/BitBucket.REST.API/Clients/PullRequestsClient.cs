using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.QueryBuilders;
using BitBucket.REST.API.Wrappers;
using RestSharp;

namespace BitBucket.REST.API.Clients
{
    public class PullRequestsClient : ApiClient
    {
        public PullRequestsClient(BitbucketRestClient restClient, BitbucketRestClient internalRestClient, Connection connection) : base(restClient, internalRestClient, connection)
        {
            
        }

        public async Task<IteratorBasedPage<PullRequest>> GetAllPullRequests(string repositoryName, string ownerName, IQueryConnector query = null)
        {
            var url = ApiUrls.PullRequests(ownerName, repositoryName);
            return await RestClient.GetAllPages<PullRequest>(url, 100, query);
        }

        public async Task<IteratorBasedPage<PullRequest>> GetPullRequests(string repositoryName)
        {
            var url = ApiUrls.PullRequests(Connection.Credentials.Login, repositoryName);
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync<IteratorBasedPage<PullRequest>>(request);
            return response.Data;
        }

        public async Task<IteratorBasedPage<UserShort>> GetAuthors(string repositoryName, string ownerName)
        {
            var url = ApiUrls.PullRequestsAuthors(ownerName, repositoryName);
            return await InternalRestClient.GetAllPages<UserShort>(url, 100);
        }

        //public async Task<IteratorBasedPage<PullRequest>> GetPullRequestsPage(string repositoryName, string ownerName, int limit = 10)
        //{
        //    var url = ApiUrls.PullRequests(ownerName, repositoryName);
        //    var request = new BitbucketRestRequest(url, Method.GET);
        //    request.AddQueryParameter("pagelen", limit.ToString());
        //    var response = await RestClient.ExecuteTaskAsync<IteratorBasedPage<PullRequest>>(request);
        //    return response.Data;
        //}

        public async Task<IteratorBasedPage<PullRequest>> GetPullRequestsPage(string repositoryName, string ownerName, int limit = 10, IQueryConnector query = null, int page = 1)
        {
            var url = ApiUrls.PullRequests(ownerName, repositoryName);
            var request = new BitbucketRestRequest(url, Method.GET);
            request.AddQueryParameter("pagelen", limit.ToString());
            request.AddQueryParameter("page", page.ToString());
            if (query != null)
            {
                request.AddQueryParameter("q", query.Build());
            }
            var response = await RestClient.ExecuteTaskAsync<IteratorBasedPage<PullRequest>>(request);
            return response.Data;
        }



        public async Task<IteratorBasedPage<PullRequest>> GetPullRequests(string repositoryName, PullRequestOptions option)
        {
            var url = ApiUrls.PullRequests(Connection.Credentials.Login, repositoryName);
            var request = new BitbucketRestRequest(url, Method.GET);
            request.AddQueryParameter("state", option.ToString());
            var response = await RestClient.ExecuteTaskAsync<IteratorBasedPage<PullRequest>>(request);
            return response.Data;
        }

        public async Task<IteratorBasedPage<PullRequest>> GetPullRequests(string repositoryName, PullRequestOptions option, int limit)
        {
            var url = ApiUrls.PullRequests(Connection.Credentials.Login, repositoryName);
            var request = new BitbucketRestRequest(url, Method.GET);
            request.AddQueryParameter("state", option.ToString()).AddQueryParameter("pagelen", limit.ToString());
            var response = await RestClient.ExecuteTaskAsync<IteratorBasedPage<PullRequest>>(request);
            return response.Data;
        }

        public async Task<string> GetPullRequestDiff(string repositoryName, long id)
        {
            return await GetPullRequestDiff(repositoryName, Connection.Credentials.Login, id);
        }

        public async Task<string> GetPullRequestDiff(string repositoryName, string owner, long id)
        {
            var url = ApiUrls.PullRequestDiff(owner, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task<Participant> ApprovePullRequest(string repositoryName, long id)
        {
            return await ApprovePullRequest(repositoryName, Connection.Credentials.Login, id);
        }

        public async Task<Participant> ApprovePullRequest(string repositoryName, string ownerName, long id)
        {
            var url = ApiUrls.PullRequestApprove(ownerName, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.POST);
            var response = await RestClient.ExecuteTaskAsync<Participant>(request);
            return response.Data;
        }

        public async Task DisapprovePullRequest(string repositoryName, long id)
        {
            await DisapprovePullRequest(repositoryName, Connection.Credentials.Login, id);
        }

        public async Task DisapprovePullRequest(string repositoryName, string ownerName, long id)
        {
            var url = ApiUrls.PullRequestApprove(ownerName, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.DELETE);
            await RestClient.ExecuteTaskAsync(request);
        }

        public async Task<IteratorBasedPage<Commit>> GetPullRequestCommits(string repositoryName, long id)
        {
            return await GetPullRequestCommits(repositoryName, Connection.Credentials.Login, id);
        }

        public async Task<IteratorBasedPage<Commit>> GetPullRequestCommits(string repositoryName, string ownerName, long id)
        {
            var url = ApiUrls.PullRequestCommits(ownerName, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync<IteratorBasedPage<Commit>>(request);
            foreach (var commit in response.Data.Values)
            {
                commit.CommitHref = $"{Connection.GetBitbucketUrl()}/{ownerName}/{repositoryName}/commits/{commit.Hash}";
            }
            return response.Data;
        }

        public async Task<IteratorBasedPage<Comment>> GetPullRequestComments(string repositoryName, long id)
        {
            return await GetPullRequestComments(repositoryName, Connection.Credentials.Login, id);
        }

        public async Task<IteratorBasedPage<Comment>> GetPullRequestComments(string repositoryName, string ownerName, long id)
        {
            var url = ApiUrls.PullRequestComments(ownerName, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync<IteratorBasedPage<Comment>>(request);
            return response.Data;
        }

  

        public async Task<PullRequest> GetPullRequest(string repositoryName, long id)
        {
            var url = ApiUrls.PullRequest(Connection.Credentials.Login, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync<PullRequest>(request);
            return response.Data;
        }

        public async Task<PullRequest> CreatePullRequest(PullRequest pullRequest, string repositoryName)
        {
            pullRequest.Author = new User()
            {
                Username = Connection.Credentials.Login
            };
            var url = ApiUrls.PullRequests(Connection.Credentials.Login, repositoryName);
            var request = new BitbucketRestRequest(url, Method.POST);
            request.AddParameter("application/json; charset=utf-8", request.JsonSerializer.Serialize(pullRequest), ParameterType.RequestBody);
            var response = await RestClient.ExecuteTaskAsync<PullRequest>(request);
            return response.Data;
        }


    }
}