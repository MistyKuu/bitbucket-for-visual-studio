using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IGitClientService
    {
        string Title { get; }
        string Origin { get; }
        Task LoginAsync(string login, string password);
        void Logout();
        Task<IEnumerable<GitRemoteRepository>> GetUserRepositoriesAsync();
        Task<IEnumerable<GitRemoteRepository>> GetAllRepositories();
        Task<GitRemoteRepository> CreateRepositoryAsync(GitRemoteRepository newRepository);
        Task<IEnumerable<GitBranch>> GetBranches(string repoName, string owner);


        Task<IEnumerable<GitTeam>> GetTeams();

        Task<IEnumerable<GitCommit>> GetPullRequestCommits(string repositoryName, string ownerName, long id);

        Task<IEnumerable<GitComment>> GetPullRequestComments(string repositoryName, string ownerName, long id);
   

        Task<PageIterator<GitPullRequest>> GetPullRequests(string repositoryName, string ownerName, int limit = 20, int page = 1);

        Task<string> GetPullRequestDiff(string repositoryName, string ownerName, long id);
        Task<string> GetPullRequestDiff(string repositoryName, long id);

        Task DisapprovePullRequest(string repositoryName, string ownerName, long id);
        Task<bool> ApprovePullRequest(string repositoryName, string ownerName, long id);

        Task<IEnumerable<GitUser>> GetPullRequestsAuthors(string repositoryName, string ownerName);

        bool IsOriginRepo(GitRemoteRepository gitRemoteRepository);

        Task<GitPullRequest> CreatePullRequest(GitPullRequest gitPullRequest, string repositoryName, string owner);
        Task<IEnumerable<GitPullRequest>> GetAllPullRequests(string repositoryName, string ownerName);
        Task<GitPullRequest> GetPullRequest(string repositoryName, string ownerName, long id);
    }
}
