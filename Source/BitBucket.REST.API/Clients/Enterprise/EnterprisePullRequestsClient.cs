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
using ParseDiff;
using RestSharp;

namespace BitBucket.REST.API.Clients.Enterprise
{
    public class EnterprisePullRequestsClient : ApiClient, IPullRequestsClient
    {
        public EnterprisePullRequestsClient(IEnterpriseBitbucketRestClient restClient, Connection connection) : base(restClient, connection)
        {

        }

        public async Task<IEnumerable<UserShort>> GetAuthors(string repositoryName, string ownerName)
        {
            var url = EnterpriseApiUrls.PullRequestsAuthors(ownerName, repositoryName);
            var users = await RestClient.GetAllPages<EnterpriseUser>(url, 100);
            return users.MapTo<List<UserShort>>();
        }

        public async Task<IteratorBasedPage<PullRequest>> GetPullRequestsPage(string repositoryName, string ownerName, int page, int limit = 50,
            IPullRequestQueryBuilder builder = null)
        {
            var url = EnterpriseApiUrls.PullRequests(ownerName, repositoryName);
            var request = new BitbucketRestRequest(url, Method.GET);

            if (builder != null)
                foreach (var param in builder.GetQueryParameters())
                    request.AddQueryParameter(param.Key, param.Value);


            request.AddQueryParameter("limit", limit.ToString()).AddQueryParameter("start", ((page - 1) * limit).ToString());

            var res = (await RestClient.ExecuteTaskAsync<EnterpriseIteratorBasedPage<EnterprisePullRequest>>(request)).Data;
            return new IteratorBasedPage<PullRequest>()
            {
                Values = res.Values.MapTo<List<PullRequest>>() //todo add rest
            };
        }

        public async Task<IEnumerable<PullRequest>> GetPullRequests(string repositoryName, string ownerName, int limit = 50, IPullRequestQueryBuilder queryBuilder = null)
        {
            var url = EnterpriseApiUrls.PullRequests(ownerName, repositoryName);

            var queryString = new QueryString();

            if (queryBuilder != null)
                foreach (var param in queryBuilder.GetQueryParameters())
                    queryString.Add(param.Key, param.Value);

            return (await RestClient.GetAllPages<EnterprisePullRequest>(url, limit, queryString))
                .MapTo<List<PullRequest>>();
        }

        public async Task<IEnumerable<FileDiff>> GetPullRequestDiff(string repositoryName, string owner, long id)
        {
            var url = EnterpriseApiUrls.PullRequestDiff(owner, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.GET);

            var response = await RestClient.ExecuteTaskAsync<EnterpriseDiffResponse>(request);
            var fileDiffs = ParseFileDiffs(response);

            return fileDiffs;
        }


        public async Task<IEnumerable<FileDiff>> GetCommitsDiff(string repoName, string owner, string fromCommit, string toCommit)
        {
            var url = EnterpriseApiUrls.CommitsDiff(owner, repoName);

            var queryString = new QueryString()
            {
                {"from", fromCommit   },
                {"to", toCommit},
            };

            var request = new BitbucketRestRequest(url, Method.GET);
            foreach (var param in queryString)
                request.AddQueryParameter(param.Key, param.Value);

            var response = await RestClient.ExecuteTaskAsync<EnterpriseDiffResponse>(request);
            var fileDiffs = ParseFileDiffs(response);

            return fileDiffs;
        }

        public async Task<Participant> ApprovePullRequest(string repositoryName, string ownerName, long id)
        {
            var url = EnterpriseApiUrls.PullRequestApprove(ownerName, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.POST);
            var response = await RestClient.ExecuteTaskAsync<EnterpriseParticipant>(request);
            return response.Data.MapTo<Participant>();
        }

        public async Task DisapprovePullRequest(string repositoryName, string ownerName, long id)
        {
            var url = EnterpriseApiUrls.PullRequestApprove(ownerName, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.DELETE);
            await RestClient.ExecuteTaskAsync(request);
        }

        public async Task DeclinePullRequest(string repositoryName, string ownerName, long id, string version)
        {
            var url = EnterpriseApiUrls.PullRequestDecline(ownerName, repositoryName, id, version);
            var request = new BitbucketRestRequest(url, Method.POST);
            await RestClient.ExecuteTaskAsync(request);
        }

        public async Task MergePullRequest(string repositoryName, string ownerName, MergeRequest mergeRequest)
        {
            var url = EnterpriseApiUrls.PullRequestMerge(ownerName, repositoryName, mergeRequest.Id, mergeRequest.Version);
            var request = new BitbucketRestRequest(url, Method.POST);
            await RestClient.ExecuteTaskAsync(request);
        }

        public Task<IEnumerable<UserShort>> GetDefaultReviewers(string repositoryName, string ownerName)
        {
            return Task.FromResult(Enumerable.Empty<UserShort>()); //todo in different API do later: http://localhost:7990/rest/default-reviewers/latest/projects/~MISTYK/repos/tttttttttt-df/reviewers?sourceRepoId=72&sourceRefId=refs%2Fheads%2FSECOND-BRANCH&targetRepoId=72&targetRefId=refs%2Fheads%2Fmaster
        }

        public IPullRequestQueryBuilder GetPullRequestQueryBuilder()
        {
            return new EnterprisePullRequestQueryBuilder();
        }

        public async Task<IEnumerable<Commit>> GetPullRequestCommits(string repositoryName, string ownerName, long id)
        {
            var url = EnterpriseApiUrls.PullRequestCommits(ownerName, repositoryName, id);
            var data = await RestClient.GetAllPages<EnterpriseCommit>(url);
            var commits = data.MapTo<List<Commit>>();

            var authors = (await GetAuthors(repositoryName, ownerName)).ToList();

            foreach (var commit in commits)
            {
                commit.CommitHref = $"{Connection.MainUrl}projects/{ownerName}/repos/{repositoryName}/pull-requests/{id}/commits/{commit.Hash}";
                if (commit.Author.User.Links.Avatar == null)
                    SetAuthorAvatar(commit, authors);
            }

            return commits;
        }

        private void SetAuthorAvatar(Commit commit, List<UserShort> participants)
        {

            try
            {
                var participant = participants.FirstOrDefault(x => x.Email == commit.Author.User.Email);

                if (participant != null)
                    commit.Author.User.Links.Avatar = new Link
                    {
                        Href = participant.Links.Self.Href + "/avatar.png"
                    };
            }
            catch (Exception e)
            {
            }
        }

        public async Task<IEnumerable<Comment>> GetPullRequestComments(string repositoryName, long id)
        {
            return await GetPullRequestComments(repositoryName, Connection.Credentials.Login, id);
        }

        public async Task<IEnumerable<Comment>> GetPullRequestComments(string repositoryName, string ownerName, long id)
        {
            var url = EnterpriseApiUrls.PullRequestActivities(ownerName, repositoryName, id);
            var activities = await RestClient.GetAllPages<EnterpriseCommentActivity>(url);
            var comments = activities.Where(x => x.Action == "COMMENTED").Select(x => x.Comment).ToList();


            foreach (var comment in comments)
                AssignCommentParent(comment);


            comments = comments.Flatten(x => true, x => x.Comments).ToList();
            return comments.MapTo<List<Comment>>();
        }
        public async Task<PullRequest> GetPullRequest(string repositoryName, string owner, long id)
        {
            var url = EnterpriseApiUrls.PullRequest(owner, repositoryName, id);
            var request = new BitbucketRestRequest(url, Method.GET);
            var pullRequest = await RestClient.ExecuteTaskAsync<EnterprisePullRequest>(request);
            return pullRequest.Data.MapTo<PullRequest>();
        }

        public async Task<PullRequest> GetPullRequestForBranches(string repositoryName, string ownerName, string sourceBranch, string destBranch)
        {
            var url = EnterpriseApiUrls.PullRequests(ownerName, repositoryName);
            var response = await RestClient.GetAllPages<EnterprisePullRequest>(url, query: new QueryString() {
            {  "at", "refs/heads/" + destBranch},
                {"state","open" }
            });
            return response.FirstOrDefault(x => x.Source.DisplayId == sourceBranch)?.MapTo<PullRequest>();
        }

        public async Task CreatePullRequest(PullRequest pullRequest, string repositoryName, string owner)
        {
            pullRequest.Author = new User()
            {
                Username = Connection.Credentials.Login
            };
            var url = EnterpriseApiUrls.PullRequests(owner, repositoryName);
            var request = new BitbucketRestRequest(url, Method.POST);
            request.AddParameter("application/json; charset=utf-8", request.JsonSerializer.Serialize(pullRequest.MapTo<EnterprisePullRequest>()), ParameterType.RequestBody);
            await RestClient.ExecuteTaskAsync(request);
        }

        public async Task UpdatePullRequest(PullRequest pullRequest, string repoName, string owner)
        {
            pullRequest.Author = new User()
            {
                Username = Connection.Credentials.Login
            };

            pullRequest.Destination = null;//throws exception if the same dest is set. Unless we allow to change it, leave it null

            var url = EnterpriseApiUrls.PullRequest(owner, repoName, pullRequest.Id);
            var request = new BitbucketRestRequest(url, Method.PUT);
            request.AddParameter("application/json; charset=utf-8", request.JsonSerializer.Serialize(pullRequest.MapTo<EnterprisePullRequest>()), ParameterType.RequestBody);
            await RestClient.ExecuteTaskAsync(request);
        }

        public async Task<IEnumerable<UserShort>> GetRepositoryUsers(string repositoryName, string ownerName, string filter)
        {
            var url = EnterpriseApiUrls.Users();
            var queryString = new QueryString()
            {
                {"permission","LICENSED_USER" },
                {"permission.1","REPO_READ" },
                {"permission.1.projectKey",ownerName },
                {"permission.1.repositorySlug",repositoryName },
                {"filter",filter }
            };

            var response = await RestClient.GetAllPages<EnterpriseUser>(url, query: queryString);

            return response.MapTo<List<UserShort>>();
        }



        private static void AssignCommentParent(EnterpriseComment parent)
        {
            foreach (var child in parent.Comments)
            {
                child.Parent = new EnterpriseParent() { Id = parent.Id };
                AssignCommentParent(child);
            }
        }

        private static List<FileDiff> ParseFileDiffs(IRestResponse<EnterpriseDiffResponse> response)
        {
            var fileDiffs = new List<FileDiff>();
            foreach (var diff in response.Data.Diffs)
            {
                var fileDiff = new FileDiff
                {
                    From = diff.Source?.String,
                    To = diff.Destination?.String,
                    Chunks = new List<ChunkDiff>(),
                };

                fileDiff.Type = fileDiff.From == null
                    ? FileChangeType.Add
                    : fileDiff.To == null ? FileChangeType.Delete : FileChangeType.Modified;

                fileDiffs.Add(fileDiff);

                foreach (var diffHunk in diff.Hunks ?? new List<Hunk>())
                {
                    var chunkDiff = new ChunkDiff()
                    {
                        Changes = new List<LineDiff>(),
                    };
                    fileDiff.Chunks.Add(chunkDiff);

                    foreach (var segment in diffHunk.Segments)
                    {
                        foreach (var line in segment.Lines)
                        {
                            var ld = new LineDiff()
                            {
                                Content = line.Text,
                                OldIndex = segment.Type == "ADDED" ? null : (int?)line.Source,
                                NewIndex = segment.Type == "REMOVED" ? null : (int?)line.Destination,
                                Type = segment.Type == "ADDED"
                                    ? LineChangeType.Add
                                    : segment.Type == "REMOVED"
                                        ? LineChangeType.Delete
                                        : LineChangeType.Normal
                            };

                            chunkDiff.Changes.Add(ld);
                        }
                    }
                }


                fileDiff.Additions = fileDiff.Chunks.Sum(y => y.Changes.Count(z => z.Add));
                fileDiff.Deletions = fileDiff.Chunks.Sum(y => y.Changes.Count(z => z.Delete));
            }
            return fileDiffs;
        }
    }
}