using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Mappings;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.QueryBuilders;
using BitBucket.REST.API.Wrappers;
using ParseDiff;
using RestSharp;

namespace BitBucket.REST.API.Clients.Standard
{
    public class PullRequestsClient : ApiClient, IPullRequestsClient
    {
        private readonly IBitbucketRestClient _internalRestClient;
        private readonly IBitbucketRestClient _webClient;

        public PullRequestsClient(
            IBitbucketRestClient restClient, 
            IBitbucketRestClient internalRestClient, 
            IBitbucketRestClient webClient,
            Connection connection) : base(restClient, connection)
        {
            _internalRestClient = internalRestClient;
            _webClient = webClient;
        }

        public async Task<IEnumerable<UserShort>> GetAuthors(string repositoryName, string ownerName)
        {
            var url = ApiUrls.PullRequestsAuthors(ownerName, repositoryName);
            return await _internalRestClient.GetAllPages<UserShort>(url, 100);
        }

        public async Task<IteratorBasedPage<PullRequest>> GetPullRequestsPage(string repositoryName, string ownerName, int page, int limit = 50,
            IPullRequestQueryBuilder builder = null)
        {
            var url = ApiUrls.PullRequests(ownerName, repositoryName);
            var request = new BitbucketRestRequest(url, Method.GET);

            if (builder != null)
                foreach (var param in builder.GetQueryParameters())
                    request.AddQueryParameter(param.Key, param.Value);

            request.AddQueryParameter("pagelen", limit.ToString()).AddQueryParameter("page", page.ToString());

            var response = await RestClient.ExecuteTaskAsync<IteratorBasedPage<PullRequest>>(request);
            return response.Data;
        }

        public async Task<IEnumerable<PullRequest>> GetPullRequests(string repositoryName, string ownerName, int limit = 50, IPullRequestQueryBuilder queryBuilder = null)
        {
            var url = ApiUrls.PullRequests(ownerName, repositoryName);

            var queryString = new QueryString();

            if (queryBuilder != null)
                foreach (var param in queryBuilder.GetQueryParameters())
                    queryString.Add(param.Key, param.Value);

            return await RestClient.GetAllPages<PullRequest>(url, limit, queryString);
        }

        public async Task<IEnumerable<FileDiff>> GetPullRequestDiff(string repositoryName, string owner, long id)
        {
            var url = ApiUrls.PullRequestDiff(owner, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync(request);
            return DiffFileParser.Parse(response.Content);
        }

        public async Task<Participant> ApprovePullRequest(string repositoryName, string ownerName, long id)
        {
            var url = ApiUrls.PullRequestApprove(ownerName, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.POST);
            var response = await RestClient.ExecuteTaskAsync<Participant>(request);
            return response.Data;
        }

        public async Task DisapprovePullRequest(string repositoryName, string ownerName, long id)
        {
            var url = ApiUrls.PullRequestApprove(ownerName, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.DELETE);
            await RestClient.ExecuteTaskAsync(request);
        }

        public async Task DeclinePullRequest(string repositoryName, string ownerName, long id, string version)
        {
            var url = ApiUrls.PullRequestDecline(ownerName, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.POST);
            await RestClient.ExecuteTaskAsync(request);
        }

        public async Task MergePullRequest(string repositoryName, string ownerName, MergeRequest mergeRequest)
        {
            var url = ApiUrls.PullRequestMerge(ownerName, repositoryName, mergeRequest.Id);
            var request = new BitbucketRestRequest(url, Method.POST);
            request.AddParameter("application/json; charset=utf-8", request.JsonSerializer.Serialize(mergeRequest), ParameterType.RequestBody);
            await RestClient.ExecuteTaskAsync(request);
        }

        public async Task<IEnumerable<UserShort>> GetDefaultReviewers(string repositoryName, string ownerName)
        {
            var url = ApiUrls.DefaultReviewers(ownerName, repositoryName);
            return await RestClient.GetAllPages<UserShort>(url, 100);
        }

        public IPullRequestQueryBuilder GetPullRequestQueryBuilder()
        {
            return new PullRequestQueryBuilder();
        }

        public async Task<IEnumerable<Commit>> GetPullRequestCommits(string repositoryName, string ownerName, long id)
        {
            var url = ApiUrls.PullRequestCommits(ownerName, repositoryName, id);
            var commits = await RestClient.GetAllPages<Commit>(url);
            foreach (var commit in commits)
            {
                commit.CommitHref = $"{Connection.MainUrl}{ownerName}/{repositoryName}/commits/{commit.Hash}";
            }
            return commits;
        }

        public async Task<IEnumerable<Comment>> GetPullRequestComments(string repositoryName, long id)
        {
            return await GetPullRequestComments(repositoryName, Connection.Credentials.Login, id);
        }

        public async Task<IEnumerable<Comment>> GetPullRequestComments(string repositoryName, string ownerName, long id)
        {
            var url = ApiUrls.PullRequestComments(ownerName, repositoryName, id);
            return await RestClient.GetAllPages<Comment>(url);
        }

        public async Task<PullRequest> GetPullRequest(string repositoryName, string owner, long id)
        {
            var url = ApiUrls.PullRequest(owner, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync<PullRequest>(request);
            return response.Data;
        }

        public async Task<PullRequest> GetPullRequestForBranches(string repositoryName, string ownerName, string sourceBranch, string destBranch)
        {
            var url = ApiUrls.PullRequests(ownerName, repositoryName);

            var queryBuilderString = new QueryBuilder()
                .Add("source.branch.name", sourceBranch)
                .Add("destination.branch.name", destBranch)
                .State(PullRequestOptions.OPEN)
                .Build();

            var query = new QueryString()
            {
                {"q", queryBuilderString},
            };
            var response = await RestClient.GetAllPages<PullRequest>(url, query: query, limit: 20);
            var pq = response.SingleOrDefault();
            if (pq == null)
                return null;

            return await GetPullRequest(repositoryName, ownerName, pq.Id); // we need reviewers and participants
        }

        public async Task<IEnumerable<FileDiff>> GetCommitsDiff(string repoName, string owner, string fromCommit, string toCommit)
        {
            if (fromCommit == toCommit) //otherwise it produces diff against its parent
                return Enumerable.Empty<FileDiff>();

            var url = ApiUrls.CommitsDiff(owner, repoName, fromCommit, toCommit);
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync(request);
            return DiffFileParser.Parse(response.Content);
        }

        public async Task CreatePullRequest(PullRequest pullRequest, string repositoryName, string owner)
        {
            pullRequest.Author = new User()
            {
                Username = Connection.Credentials.Login
            };

            var url = ApiUrls.PullRequests(owner, repositoryName);
            var request = new BitbucketRestRequest(url, Method.POST);
            request.AddParameter("application/json; charset=utf-8", request.JsonSerializer.Serialize(pullRequest), ParameterType.RequestBody);
            var response = await RestClient.ExecuteTaskAsync<PullRequest>(request);
        }


        public async Task UpdatePullRequest(PullRequest pullRequest, string repoName, string owner)
        {
            pullRequest.Author = new User()
            {
                Username = Connection.Credentials.Login
            };

            var url = ApiUrls.PullRequest(owner, repoName, pullRequest.Id);
            var request = new BitbucketRestRequest(url, Method.PUT);
            request.AddParameter("application/json; charset=utf-8", request.JsonSerializer.Serialize(pullRequest), ParameterType.RequestBody);
            var response = await RestClient.ExecuteTaskAsync<PullRequest>(request);
        }


        public async Task<IEnumerable<UserShort>> GetRepositoryUsers(string repositoryName, string ownerName, string filter)
        {
            var url = ApiUrls.Mentions(ownerName, repositoryName);
            var query = new QueryString()
            {
                {"term",filter }
            };

            var request = new BitbucketRestRequest(url, Method.GET);

            foreach (var par in query)
                request.AddQueryParameter(par.Key, par.Value);

            var response = await _webClient.ExecuteTaskAsync<List<UserShort>>(request);
            return response.Data;
        }
    }
}