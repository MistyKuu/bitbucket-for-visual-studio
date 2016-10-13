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
        Task<IEnumerable<GitRepository>> GetAllRepositories();
        Task<GitRepository> CreateRepositoryAsync(GitRepository newRepository);
        Task<IEnumerable<GitBranch>> GetBranches(GitRepository repo);
        Task<IEnumerable<GitCommit>> GetPullRequestCommits(GitRepository repo, long id);
        Task<IEnumerable<GitComment>> GetPullRequestComments(GitRepository repo, long id);


        Task<PageIterator<GitPullRequest>> GetPullRequests(GitRepository repo, int limit = 20, int page = 1);

        Task<string> GetPullRequestDiff(GitRepository repo, long id);

        Task DisapprovePullRequest(GitRepository repo, long id);
        Task<bool> ApprovePullRequest(GitRepository repo, long id);

        Task<IEnumerable<GitUser>> GetPullRequestsAuthors(GitRepository repo);

        bool IsOriginRepo(GitRepository gitRepository);

        Task<GitPullRequest> CreatePullRequest(GitPullRequest gitPullRequest, GitRepository repo);
        Task<GitPullRequest> GetPullRequest(GitRepository repo, long id);
        Task<IEnumerable<GitTeam>> GetTeams();
    }
}
