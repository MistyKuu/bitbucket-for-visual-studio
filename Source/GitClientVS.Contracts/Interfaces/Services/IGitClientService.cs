using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using ParseDiff;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IGitClientService
    {
        string GitClientType { get; }
        string Title { get; }
        string Origin { get; }
        Task LoginAsync(GitCredentials gitCredentials);
        void Logout();
        Task<IEnumerable<GitRemoteRepository>> GetAllRepositories();
        Task<GitRemoteRepository> CreateRepositoryAsync(GitRemoteRepository newRepository);
        Task<IEnumerable<GitBranch>> GetBranches();
        Task<IEnumerable<GitTeam>> GetTeams();
        Task<IEnumerable<GitCommit>> GetPullRequestCommits(long id);
        Task<IEnumerable<GitComment>> GetPullRequestComments(long id);
        Task<IEnumerable<FileDiff>> GetPullRequestDiff(long id);
        Task DisapprovePullRequest(long id);
        Task<bool> ApprovePullRequest(long id);
        Task<bool> DeclinePullRequest(long id, string version);
        Task<bool> MergePullRequest(GitMergeRequest request);
        Task<IEnumerable<GitUser>> GetPullRequestsAuthors();
        bool IsOriginRepo(GitRemoteRepository gitRemoteRepository);
        Task CreatePullRequest(GitPullRequest gitPullRequest);
        Task<GitPullRequest> GetPullRequest(long id);
        Task<IEnumerable<GitUser>> GetRepositoryUsers(string filter);
        Task<GitPullRequest> GetPullRequestForBranches(string sourceBranch, string destBranch);
        Task<GitCommit> GetCommitById(string id);
        Task<IEnumerable<GitCommit>> GetCommitsRange(GitBranch fromBranch, GitBranch toBranch);
        Task<IEnumerable<FileDiff>> GetCommitsDiff(string fromCommit, string toCommit);
        Task UpdatePullRequest(GitPullRequest gitPullRequest);
        Task<IEnumerable<GitUser>> GetDefaultReviewers();
        Task<string> GetFileContent(string hash, string path);

        Task AddPullRequestComment(GitComment comment);

        Task DeletePullRequestComment(long id);

        Task<IEnumerable<GitPullRequest>> GetPullRequests(
            int limit = 50,
            GitPullRequestStatus? state = null,
            string fromBranch = null,
            string toBranch = null,
            bool isDescSorted = true,
            string author = null
        );

        Task<PageIterator<GitPullRequest>> GetPullRequestsPage(
           int page,
           int limit = 50,
           GitPullRequestStatus? state = null,
           string fromBranch = null,
           string toBranch = null,
           bool isDescSorted = true,
           string author = null
       );
    }
}
