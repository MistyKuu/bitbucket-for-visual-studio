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

        [ImportingConstructor]
        public BitbucketService(IEventAggregatorService eventAggregator)
        {
            _eventAggregator = eventAggregator;
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

        public async Task<IEnumerable<GitTeam>> GetTeams()
        {
            var teams = await _bitbucketClient.TeamsClient.GetTeams();
            return teams.Values.MapTo<List<GitTeam>>();
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
            var pullRequests = await _bitbucketClient.PullRequestsClient.GetPullRequestsPage(repositoryName, ownerName, 50);
            return pullRequests.Values.MapTo<List<GitPullRequest>>();
        }

        public async Task<IEnumerable<GitPullRequest>> GetPullRequestsAfterDate(string repositoryName, string ownerName)
        {
            //todo put real repository name
            var fakeDate = DateTime.ParseExact("2016-08-01", "yyyy-MM-dd",
                                  CultureInfo.InvariantCulture);
            PullRequestQueryBuilder queryBuilder = new PullRequestQueryBuilder();
            var readyQuery = queryBuilder.StartBuilding().CreatedOn(fakeDate, Operators.Greater);
            //with state
            // queryBuilder.StartBuilding().CreatedOn(fakeDate, Operators.Greater).And().State(PullRequestOptions.MERGED);

            var pullRequests = await _bitbucketClient.PullRequestsClient.GetPullRequestsPage(repositoryName, ownerName, 50, readyQuery);
            return pullRequests.Values.MapTo<List<GitPullRequest>>();
        }

        public async Task<IEnumerable<GitPullRequest>> GetPullRequests(GitPullRequestStatus gitPullRequestStatus, string repositoryName)
        {  
            var pullRequests = await _bitbucketClient.PullRequestsClient.GetPullRequests(repositoryName, gitPullRequestStatus.MapTo<PullRequestOptions>());
            return pullRequests.Values.MapTo<List<GitPullRequest>>();
        }

        public async Task<IEnumerable<GitBranch>> GetBranches(string repoName)
        {
            var repositories = await _bitbucketClient.RepositoriesClient.GetBranches(repoName);
            return repositories.Values.MapTo<List<GitBranch>>();
        }

        public async Task<bool> ApprovePullRequest(string ownerName, string repositoryName, long id)
        {
            var result = await _bitbucketClient.PullRequestsClient.ApprovePullRequest(repositoryName, ownerName, id);
            return (result != null && result.Approved);
        }

        public async Task DisapprovePullRequest(string ownerName, string repositoryName, long id)
        {
            await _bitbucketClient.PullRequestsClient.DisapprovePullRequest(repositoryName, ownerName, id);
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

        public async Task<IEnumerable<GitCommit>> GetPullRequestCommits(string repositoryName, string ownerName, long id)
        {
            var commits = await _bitbucketClient.PullRequestsClient.GetPullRequestCommits(repositoryName, ownerName, id);
            return commits.Values.MapTo<List<GitCommit>>();
        }

        public async Task<IEnumerable<GitCommit>> GetPullRequestCommits(string repositoryName, long id)
        {
            var commits = await _bitbucketClient.PullRequestsClient.GetPullRequestCommits(repositoryName, id);
            return commits.Values.MapTo<List<GitCommit>>();
        }

        public async Task<IEnumerable<GitComment>> GetPullRequestComments(string repositoryName, string ownerName, long id)
        {
            var commits = await _bitbucketClient.PullRequestsClient.GetPullRequestComments(repositoryName, ownerName, id);
            return commits.Values.MapTo<List<GitComment>>();
        }

        public async Task<IEnumerable<GitComment>> GetPullRequestComments(string repositoryName, long id)
        {
            var commits = await _bitbucketClient.PullRequestsClient.GetPullRequestComments(repositoryName, id);
            return commits.Values.MapTo<List<GitComment>>();
        }
    }
}
