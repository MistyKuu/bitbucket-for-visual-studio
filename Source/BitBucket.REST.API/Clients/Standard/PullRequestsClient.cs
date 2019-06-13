using System;
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
        public PullRequestsClient(IBitbucketRestClient restClient,
            Connection connection) : base(restClient, connection)
        {
        }

        public Task<IEnumerable<User>> GetAuthors(string repositoryName, string ownerName)
        {
            return GetRepositoryUsers(repositoryName, ownerName, null);
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

        public async Task<IEnumerable<User>> GetDefaultReviewers(string repositoryName, string ownerName)
        {
            var url = ApiUrls.DefaultReviewers(ownerName, repositoryName);
            return await RestClient.GetAllPages<User>(url, 100);
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

        public async Task<IEnumerable<Comment>> GetPullRequestComments(string repositoryName, string ownerName, long id)
        {
            var url = ApiUrls.PullRequestComments(ownerName, repositoryName, id);
            return (await RestClient.GetAllPages<Comment>(url));
        }

        public async Task<Comment> AddPullRequestComment(string repositoryName, string ownerName, long id, string content, long? lineFrom = null, long? lineTo = null, string fileName = null, long? parentId = null)
        {
            var url = ApiUrls.PullRequestComments(ownerName, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.POST);

            var body = new Comment()
            {
                Content = new Content() { Raw = content },
                Inline = fileName != null ? new Inline()
                {
                    From = parentId != null ? null : lineFrom,
                    To = parentId != null ? null : lineTo,
                    Path = fileName
                } : null,
                Parent = parentId.HasValue ? new Parent() { Id = parentId.Value } : null
            };

            request.AddParameter("application/json; charset=utf-8", request.JsonSerializer.Serialize(body), ParameterType.RequestBody);

            var response = await RestClient.ExecuteTaskAsync<Comment>(request);
            return response.Data;
        }

        public async Task DeletePullRequestComment(string repositoryName, string ownerName, long pullRequestId, long id, long version)
        {
            var url = ApiUrls.PullRequestComments(ownerName, repositoryName, pullRequestId, id);
            var request = new BitbucketRestRequest(url, Method.DELETE);
            await RestClient.ExecuteTaskAsync(request);
        }

        public async Task<Comment> EditPullRequestComment(string repositoryName, string ownerName, long pullRequestId, long id, string content, long commentVersion)
        {
            var url = ApiUrls.PullRequestComments(ownerName, repositoryName, pullRequestId, id);
            var request = new BitbucketRestRequest(url, Method.PUT);

            var body = new Comment()
            {
                Content = new Content() { Raw = content },
            };

            request.AddParameter("application/json; charset=utf-8", request.JsonSerializer.Serialize(body), ParameterType.RequestBody);

            var response = await RestClient.ExecuteTaskAsync<Comment>(request);
            return response.Data;
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

        public async Task<string> GetFileContent(string repoName, string owner, string hash, string path)
        {
            var url = ApiUrls.DownloadFile(owner, repoName, hash, path);
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync(request);
            return response.Content;
        }

        public async Task CreatePullRequest(PullRequest pullRequest, string repositoryName, string owner)
        {
            pullRequest.Author = new User()
            {
                Username = Connection.Credentials.Login,
            };

            pullRequest.Reviewers = pullRequest.Reviewers.Select(x => new User() {Uuid = x.Uuid}).ToList();

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

            pullRequest.Reviewers = pullRequest.Reviewers.Select(x => new User() { Uuid = x.Uuid }).ToList();

            var url = ApiUrls.PullRequest(owner, repoName, pullRequest.Id);
            var request = new BitbucketRestRequest(url, Method.PUT);
            request.AddParameter("application/json; charset=utf-8", request.JsonSerializer.Serialize(pullRequest), ParameterType.RequestBody);
            var response = await RestClient.ExecuteTaskAsync<PullRequest>(request);
        }


        public async Task<IEnumerable<User>> GetRepositoryUsers(string repositoryName, string ownerName, string filter)
        {
            var url = ApiUrls.RepositoryUsers(ownerName, repositoryName);

            var response = await RestClient.GetAllPages<PermissionDto>(url);

            return response
                .Where(x => filter == null || x.User.DisplayName.StartsWith(filter, true, CultureInfo.InvariantCulture))
                .Select(x => x.User)
                .ToList();
        }
    }
}