using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BitBucket.REST.API;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.QueryBuilders;
using GitClientVS.Contracts;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure;
using GitClientVS.Infrastructure.Extensions;

namespace GitClientVS.Services
{
    [Export(typeof(IGitClientService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [ExportMetadata(Consts.GitProviderKey, GitProviderType.Bitbucket)]
    public class BitbucketService : IGitClientService
    {
        private readonly IEventAggregatorService _eventAggregator;
        private BitbucketClient _bitbucketClient;

        public bool IsConnected => _bitbucketClient != null;

        [ImportingConstructor]
        public BitbucketService(IEventAggregatorService eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }


        public string Origin => "Bitbucket";
        public string Title => $"{Origin} Extension";
        private readonly string supportedSCM = "git";

        public async Task LoginAsync(string login, string password)
        {
            if (IsConnected)
                return;

            var credentials = new Credentials(login, password);
            var connection = new Connection(credentials);
            var bitbucketInitializer = new BitbucketClientInitializer(connection);
            _bitbucketClient = await bitbucketInitializer.Initialize();
            OnConnectionChanged(ConnectionData.Create(_bitbucketClient.Connection.Credentials.Login, password, Connection.DefaultBitbucketUrl.AbsoluteUri, GitProviderType.Bitbucket));
        }
        
        public async Task<IEnumerable<GitRepository>> GetAllRepositories()
        {
            var allRepositories = new List<GitRepository>();

            var userRepositories = await _bitbucketClient.RepositoriesClient.GetRepositories();
            allRepositories.AddRange(userRepositories.Values.Where(repo => repo.Scm == supportedSCM).MapTo<List<GitRepository>>());

            var teams = await _bitbucketClient.TeamsClient.GetTeams();
            foreach (var team in teams.Values)
            {
                var teamRepositories = await _bitbucketClient.RepositoriesClient.GetRepositories(team.Username);
                allRepositories.AddRange(teamRepositories.Values.Where(repo => repo.Scm == supportedSCM).MapTo<List<GitRepository>>());
            }

            return allRepositories;
        }

        public async Task<IEnumerable<GitTeam>> GetTeams()
        {
            var teams = await _bitbucketClient.TeamsClient.GetTeams();
            return teams.Values.MapTo<List<GitTeam>>();
        }


        public async Task<GitPullRequest> GetPullRequest(GitRepository repo, long id)
        {
            return (await _bitbucketClient.PullRequestsClient.GetPullRequest(repo.Name, repo.Owner, id)).MapTo<GitPullRequest>();
        }

        public async Task<string> GetPullRequestDiff(GitRepository repo, long id)
        {
            return await _bitbucketClient.PullRequestsClient.GetPullRequestDiff(repo.Name, repo.Owner, id);
        }

        public bool IsOriginRepo(GitRepository gitRepository)
        {
            if (gitRepository?.CloneUrl == null) return false;
            Uri uri = new Uri(gitRepository.CloneUrl);
            return uri.Host.Contains(_bitbucketClient.Connection.GetHost(), StringComparison.OrdinalIgnoreCase);
        }

        public async Task<GitRepository> CreateRepositoryAsync(GitRepository newRepository)
        {
            var repository = newRepository.MapTo<Repository>();
            var result = await _bitbucketClient.RepositoriesClient.CreateRepository(repository);
            return result.MapTo<GitRepository>();
        }


        public async Task<IEnumerable<GitPullRequest>> GetAllPullRequests(GitRepository repo)
        {
            //todo put real repository name
            var pullRequests = await _bitbucketClient.PullRequestsClient.GetAllPullRequests(repo.Name, repo.Owner);
            return pullRequests.Values.MapTo<List<GitPullRequest>>();
        }

        public async Task<PageIterator<GitPullRequest>> GetPullRequests(GitRepository repo, int limit = 20, int page = 1)
        {
            //todo put real repository name
            var pullRequests = await _bitbucketClient.PullRequestsClient.GetPullRequestsPage(repo.Name, repo.Owner, limit: limit, page: page);
            return pullRequests.MapTo<PageIterator<GitPullRequest>>();
        }
        

        public async Task<IEnumerable<GitBranch>> GetBranches(GitRepository repo)
        {
            var repositories = await _bitbucketClient.RepositoriesClient.GetBranches(repo.Name, repo.Owner);
            return repositories.Values.MapTo<List<GitBranch>>();
        }

        public async Task<IEnumerable<GitUser>> GetPullRequestsAuthors(GitRepository repo)
        {
            var authors = await _bitbucketClient.PullRequestsClient.GetAuthors(repo.Name, repo.Owner);
            return authors.Values.MapTo<List<GitUser>>();
        }

        public async Task<bool> ApprovePullRequest(GitRepository repo, long id)
        {
            var result = await _bitbucketClient.PullRequestsClient.ApprovePullRequest(repo.Name, repo.Owner, id);
            return (result != null && result.Approved);
        }

        public async Task DisapprovePullRequest(GitRepository repo, long id)
        {
            await _bitbucketClient.PullRequestsClient.DisapprovePullRequest(repo.Name, repo.Owner, id);
        }

        public async Task<GitPullRequest> CreatePullRequest(GitPullRequest gitPullRequest, GitRepository repo)
        {
            var responsePullRequest = await _bitbucketClient.PullRequestsClient.CreatePullRequest(gitPullRequest.MapTo<PullRequest>(), repo.Name, repo.Owner);
            return responsePullRequest.MapTo<GitPullRequest>();
        }

        public void Logout()
        {
            _bitbucketClient = null;
            OnConnectionChanged(ConnectionData.NotLogged);
        }

        private void OnConnectionChanged(ConnectionData connectionData)
        {
            _eventAggregator.Publish(new ConnectionChangedEvent(connectionData));
        }

        public async Task<IEnumerable<GitCommit>> GetPullRequestCommits(GitRepository repo, long id)
        {
            var commits = await _bitbucketClient.PullRequestsClient.GetPullRequestCommits(repo.Name, repo.Owner, id);
            return commits.Values.MapTo<List<GitCommit>>();
        }

        public async Task<IEnumerable<GitComment>> GetPullRequestComments(GitRepository repo, long id)
        {
            var commits = await _bitbucketClient.PullRequestsClient.GetPullRequestComments(repo.Name, repo.Owner, id);
            return commits.Values.MapTo<List<GitComment>>();
        }

    }
}
