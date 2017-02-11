using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Mappings;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.QueryBuilders;
using BitBucket.REST.API.Wrappers;
using RestSharp;

namespace BitBucket.REST.API.Clients.Enterprise
{
    public class EnterprisePullRequestsClient : ApiClient, IPullRequestsClient
    {
        public EnterprisePullRequestsClient(EnterpriseBitbucketRestClient restClient, Connection connection) : base(restClient, connection)
        {

        }

        public async Task<IteratorBasedPage<PullRequest>> GetAllPullRequests(string repositoryName, string ownerName, IQueryConnector query = null)
        {
            var url = EnterpriseApiUrls.PullRequests(ownerName, repositoryName);
            var pullRequests = await RestClient.GetAllPages<EnterprisePullRequest>(url, 100, query);
            return pullRequests.MapTo<IteratorBasedPage<PullRequest>>();
        }


        public async Task<IteratorBasedPage<UserShort>> GetAuthors(string repositoryName, string ownerName)
        {
            var url = EnterpriseApiUrls.PullRequestsAuthors(ownerName, repositoryName);
            var users = await RestClient.GetAllPages<EnterpriseUser>(url, 100);
            return users.MapTo<IteratorBasedPage<UserShort>>();
        }

        public async Task<IteratorBasedPage<PullRequest>> GetPullRequestsPage(string repositoryName, string ownerName, int limit = 10, IQueryConnector query = null, int page = 1)
        {
            var url = EnterpriseApiUrls.PullRequests(ownerName, repositoryName);
            var request = new BitbucketRestRequest(url, Method.GET);
            request.AddQueryParameter("limit", limit.ToString());
            request.AddQueryParameter("start", (page - 1).ToString());
            if (query != null)
            {
                request.AddQueryParameter("q", query.Build());
            }
            var response = await RestClient.ExecuteTaskAsync<IteratorBasedPage<EnterprisePullRequest>>(request);
            return response.Data.MapTo<IteratorBasedPage<PullRequest>>();
        }

        public async Task<string> GetPullRequestDiff(string repositoryName, long id)
        {
            return await GetPullRequestDiff(repositoryName, Connection.Credentials.Login, id);
        }

        public async Task<string> GetPullRequestDiff(string repositoryName, string owner, long id)
        {//todo this is returning json
            var url = EnterpriseApiUrls.PullRequestDiff(owner, repositoryName, id);
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
            throw new NotSupportedException("XLSF FAILED");
            var url = EnterpriseApiUrls.PullRequestApprove(ownerName, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.POST);
            var response = await RestClient.ExecuteTaskAsync<EnterpriseParticipant>(request);
            return response.Data.MapTo<Participant>();
        }

        public async Task DisapprovePullRequest(string repositoryName, long id)
        {
            await DisapprovePullRequest(repositoryName, Connection.Credentials.Login, id);
        }

        public async Task DisapprovePullRequest(string repositoryName, string ownerName, long id)
        {
            throw new NotSupportedException("XLSF FAILED");
            var url = EnterpriseApiUrls.PullRequestApprove(ownerName, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.DELETE);
            await RestClient.ExecuteTaskAsync(request);
        }

        public async Task<IteratorBasedPage<Commit>> GetPullRequestCommits(string repositoryName, string ownerName, long id)
        {
            var url = EnterpriseApiUrls.PullRequestCommits(ownerName, repositoryName, id);
            var data = await RestClient.GetAllPages<EnterpriseCommit>(url);
            var mapped = data.MapTo<IteratorBasedPage<Commit>>();

            foreach (var commit in mapped.Values)
                commit.CommitHref = $"{Connection.MainUrl}/{ownerName}/{repositoryName}/commits/{commit.Hash}";

            return mapped;
        }

        public async Task<IteratorBasedPage<Comment>> GetPullRequestComments(string repositoryName, long id)
        {
            return await GetPullRequestComments(repositoryName, Connection.Credentials.Login, id);
        }

        public async Task<IteratorBasedPage<Comment>> GetPullRequestComments(string repositoryName, string ownerName, long id)
        {
            var url = EnterpriseApiUrls.PullRequestActivities(ownerName, repositoryName, id);//todo limit is set to 100 only, THIS IS NOT WORKING CORRECTLY ANYWHERE
            var activities = await RestClient.GetAllPages<CommentEnterpriseActivity>(url);
            var comments = activities.Values.Where(x => x.Action == EnterpriseActivityType.COMMENTED).Select(x => x.Comment).ToList();
            //TODO this wont work as comments has other comments nested. Check how we use it and build it properly
            return new IteratorBasedPage<EnterpriseComment>()
            {
                Size = activities.Size,
                Next = activities.Next,
                PageLen = activities.PageLen,
                Page = activities.Page,
                Values = comments
            }.MapTo<IteratorBasedPage<Comment>>();
        }

        public async Task<PullRequest> GetPullRequest(string repositoryName, string owner, long id)
        {
            var url = EnterpriseApiUrls.PullRequest(owner, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.GET);
            var pullRequest = await RestClient.ExecuteTaskAsync<EnterprisePullRequest>(request);
            return pullRequest.Data.MapTo<PullRequest>();
        }

        public async Task<PullRequest> CreatePullRequest(PullRequest pullRequest, string repositoryName, string owner)
        {
            pullRequest.Author = new User()
            {
                Username = Connection.Credentials.Login
            };
            var url = EnterpriseApiUrls.PullRequests(owner, repositoryName);
            var request = new BitbucketRestRequest(url, Method.POST);
            request.AddParameter("application/json; charset=utf-8", request.JsonSerializer.Serialize(pullRequest.MapTo<EnterprisePullRequest>()), ParameterType.RequestBody);
            var response = await RestClient.ExecuteTaskAsync<EnterprisePullRequest>(request);
            return response.Data.MapTo<PullRequest>();
        }


    }
}