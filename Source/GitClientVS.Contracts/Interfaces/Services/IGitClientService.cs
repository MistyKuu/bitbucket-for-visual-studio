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
        Task<IEnumerable<GitRemoteRepository>> GetUserRepositoriesAsync();
        Task<IEnumerable<GitRemoteRepository>> GetAllRepositories();
        Task<GitRemoteRepository> CreateRepositoryAsync(GitRemoteRepository newRepository);
        Task<IEnumerable<GitBranch>> GetBranches();
        Task<IEnumerable<GitTeam>> GetTeams();
        Task<IEnumerable<GitCommit>> GetPullRequestCommits(long id);
        Task<IEnumerable<GitComment>> GetPullRequestComments(long id);
        Task<PageIterator<GitPullRequest>> GetPullRequests(int limit = 20, int page = 1);
        Task<IEnumerable<FileDiff>> GetPullRequestDiff(long id);
        Task DisapprovePullRequest(long id);
        Task<bool> ApprovePullRequest(long id);
        Task<IEnumerable<GitUser>> GetPullRequestsAuthors();
        bool IsOriginRepo(GitRemoteRepository gitRemoteRepository);
        Task CreatePullRequest(GitPullRequest gitPullRequest);
        Task<IEnumerable<GitPullRequest>> GetAllPullRequests();
        Task<GitPullRequest> GetPullRequest(long id);
        Task<IEnumerable<GitUser>> GetRepositoryUsers(string filter);
        Task<GitPullRequest> GetPullRequestForBranches(string sourceBranch, string destBranch);
        Task<GitCommit> GetCommitById(string id);
        Task<IEnumerable<GitCommit>> GetCommitsRange(GitBranch fromBranch, GitBranch toBranch);
        Task<IEnumerable<FileDiff>> GetCommitsDiff(string fromCommit, string toCommit);
        Task UpdatePullRequest(GitPullRequest gitPullRequest);
    }
}
