using System.Collections.Generic;
using System.Threading.Tasks;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.QueryBuilders;
using ParseDiff;

namespace BitBucket.REST.API.Interfaces
{
    public interface IPullRequestsClient
    {
        Task<IEnumerable<UserShort>> GetAuthors(string repositoryName, string ownerName);
        Task<IteratorBasedPage<PullRequest>> GetPullRequestsPage(string repositoryName, string ownerName, int page, int limit = 50, IPullRequestQueryBuilder builder = null);
        Task<IEnumerable<PullRequest>> GetPullRequests(string repositoryName, string ownerName, int limit = 50, IPullRequestQueryBuilder builder = null);
        Task<IEnumerable<FileDiff>> GetPullRequestDiff(string repositoryName, string owner, long id);
        Task<Participant> ApprovePullRequest(string repositoryName, string ownerName, long id);
        Task DisapprovePullRequest(string repositoryName, string ownerName, long id);
        Task<IEnumerable<Commit>> GetPullRequestCommits(string repositoryName, string ownerName, long id);
        Task<IEnumerable<Comment>> GetPullRequestComments(string repositoryName, string ownerName, long id);
        Task<PullRequest> GetPullRequest(string repositoryName, string owner, long id);
        Task CreatePullRequest(PullRequest pullRequest, string repositoryName, string owner);
        Task<IEnumerable<UserShort>> GetRepositoryUsers(string repositoryName, string ownerName, string filter);
        Task<PullRequest> GetPullRequestForBranches(string repositoryName, string ownerName, string sourceBranch, string destBranch);
        Task<IEnumerable<FileDiff>> GetCommitsDiff(string repoName, string owner, string fromCommit, string toCommit);
        Task UpdatePullRequest(PullRequest pullRequest, string repoName, string owner);
        Task DeclinePullRequest(string repositoryName, string ownerName, long id, string version);
        Task MergePullRequest(string repositoryName, string ownerName, MergeRequest mergeRequest);
        Task<IEnumerable<UserShort>> GetDefaultReviewers(string repositoryName, string ownerName);
        IPullRequestQueryBuilder GetPullRequestQueryBuilder();
        Task<string> GetFileContent(string repoName, string owner, string hash, string path);

        Task AddPullRequestComment(
            string repositoryName,
            string ownerName,
            long id,
            string content,
            long? lineFrom = null,
            long? lineTo = null,
            string fileName = null,
            long? parentId = null
        );
    }
}