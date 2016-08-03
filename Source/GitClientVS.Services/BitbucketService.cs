using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BitBucket.REST.API;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Events;
using GitClientVS.Infrastructure.Extensions;

namespace GitClientVS.Services
{
    [Export(typeof(IGitClientService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class BitbucketService : IGitClientService
    {
        private readonly IEventAggregatorService _eventAggregator;
        private BitbucketClient _bitbucketClient;

        public bool IsConnected => _bitbucketClient != null;
        private Dictionary<String, List<String>> repositoriesNamespaces;


        [ImportingConstructor]
        public BitbucketService(IEventAggregatorService eventAggregator)
        {
            _eventAggregator = eventAggregator;
            repositoriesNamespaces = new Dictionary<string, List<string>>();
        }


        public string Origin => "BitBucket";
        public string Title => $"{Origin} Extension";

        public async Task LoginAsync(string login, string password)
        {
            if (IsConnected)
                return;

            var credentials = new Credentials(login, password);
            var connection = new Connection(credentials);
            var bitbucketInitializer = new BitbucketClientInitializer(connection);
            _bitbucketClient = await bitbucketInitializer.Initialize();
            OnConnectionChanged(ConnectionData.Create(login, password));

            CreateRepositoriesNamespaces(login);
        }

        private void CreateRepositoriesNamespaces(string login)
        {
            //repositoriesNamespaces.Add(login, new List<string>());
            //await _bitbucketClient.TeamsClient.GetTeams();
        }

        public async Task<IEnumerable<GitRemoteRepository>> GetUserRepositoriesAsync()
        {
            var repositories = await _bitbucketClient.RepositoriesClient.GetRepositories();
            return repositories.Values.MapTo<List<GitRemoteRepository>>();
        }

        public async Task<IEnumerable<GitRemoteRepository>> GetAllRepositories()
        {
            var allRepositories = new List<GitRemoteRepository>();

            var userRepositories = await _bitbucketClient.RepositoriesClient.GetRepositories();
            allRepositories.AddRange(userRepositories.Values.MapTo<List<GitRemoteRepository>>());

            var teams = await _bitbucketClient.TeamsClient.GetTeams();
            foreach (var team in teams.Values)
            {
                var teamRepositories = await _bitbucketClient.RepositoriesClient.GetRepositories(team.Username);
                allRepositories.AddRange(teamRepositories.Values.MapTo<List<GitRemoteRepository>>());
            }

            return allRepositories;
        }

        public async Task<string> GetPullRequestDiff(string repositoryName, long id)
        {
            return await _bitbucketClient.PullRequestsClient.GetPullRequestDiff(repositoryName, id);
        }

        public async Task<string> GetPullRequestDiff(string repositoryName, string ownerName, long id)
        {
            return await _bitbucketClient.PullRequestsClient.GetPullRequestDiff(repositoryName, ownerName, id);
        }

        public bool IsOriginRepo(GitRemoteRepository gitRemoteRepository)
        {
            Uri uri = new Uri(gitRemoteRepository.CloneUrl);
            return uri.Host.Contains(_bitbucketClient.GetHost(), StringComparison.OrdinalIgnoreCase);
        }

        public async Task<GitRemoteRepository> CreateRepositoryAsync(GitRemoteRepository newRepository)
        {
            var repository = newRepository.MapTo<Repository>();
            var result = await _bitbucketClient.RepositoriesClient.CreateRepository(repository);
            return result.MapTo<GitRemoteRepository>();
        }

        public async Task<IEnumerable<GitPullRequest>> GetPullRequests(string repositoryName)
        {
            //todo put real repository name
            var pullRequests = await _bitbucketClient.PullRequestsClient.GetPullRequests(repositoryName);
            return pullRequests.Values.MapTo<List<GitPullRequest>>();
        }

        public async Task<IEnumerable<GitPullRequest>> GetPullRequests(string repositoryName, string ownerName)
        {
            //todo put real repository name
            var pullRequests = await _bitbucketClient.PullRequestsClient.GetPullRequests(repositoryName, ownerName);
            return pullRequests.Values.MapTo<List<GitPullRequest>>();
        }

        public async Task<IEnumerable<GitPullRequest>> GetPullRequests(GitPullRequestStatus gitPullRequestStatus, string repositoryName)
        {
            PullRequestOptions option = PullRequestOptions.OPEN;
            if (gitPullRequestStatus == GitPullRequestStatus.Declined)
            {
                option = PullRequestOptions.DECLINED;
            }
            else if (gitPullRequestStatus == GitPullRequestStatus.Merged)
            {
                option = PullRequestOptions.MERGED;
            }
            else if (gitPullRequestStatus == GitPullRequestStatus.Open)
            {
                option = PullRequestOptions.OPEN;
            }
            // todo: how better   
            var pullRequests = await _bitbucketClient.PullRequestsClient.GetPullRequests(repositoryName, option);
            return pullRequests.Values.MapTo<List<GitPullRequest>>();
        }

        public async Task<IEnumerable<GitBranch>> GetBranches(string repoName)
        {
            var repositories = await _bitbucketClient.RepositoriesClient.GetBranches(repoName);
            return repositories.Values.MapTo<List<GitBranch>>();
        }

        public async Task<GitPullRequest> CreatePullRequest(GitPullRequest gitPullRequest, string repositoryName)
        {
            var responsePullRequest = await _bitbucketClient.PullRequestsClient.CreatePullRequest(gitPullRequest.MapTo<PullRequest>(), repositoryName);
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
    }
}
