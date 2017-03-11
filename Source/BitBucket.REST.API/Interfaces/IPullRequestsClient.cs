using System.Collections.Generic;
using System.Threading.Tasks;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.QueryBuilders;
using ParseDiff;

namespace BitBucket.REST.API.Interfaces
{
    public interface IPullRequestsClient
    {
        Task<IEnumerable<PullRequest>> GetAllPullRequests(string repositoryName, string ownerName, IQueryConnector query = null);
        Task<IEnumerable<UserShort>> GetAuthors(string repositoryName, string ownerName);
        Task<IteratorBasedPage<PullRequest>> GetPullRequestsPage(string repositoryName, string ownerName, int limit = 10, IQueryConnector query = null, int page = 1);
        Task<IEnumerable<FileDiff>> GetPullRequestDiff(string repositoryName, long id);
        Task<IEnumerable<FileDiff>> GetPullRequestDiff(string repositoryName, string owner, long id);
        Task<Participant> ApprovePullRequest(string repositoryName, long id);
        Task<Participant> ApprovePullRequest(string repositoryName, string ownerName, long id);
        Task DisapprovePullRequest(string repositoryName, long id);
        Task DisapprovePullRequest(string repositoryName, string ownerName, long id);
        Task<IEnumerable<Commit>> GetPullRequestCommits(string repositoryName, string ownerName, long id);
        Task<IEnumerable<Comment>> GetPullRequestComments(string repositoryName, long id);
        Task<IEnumerable<Comment>> GetPullRequestComments(string repositoryName, string ownerName, long id);
        Task<PullRequest> GetPullRequest(string repositoryName, string owner, long id);
        Task CreatePullRequest(PullRequest pullRequest, string repositoryName, string owner);
    }
}