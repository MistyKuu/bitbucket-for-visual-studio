using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BitBucket.REST.API;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.QueryBuilders;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure;
using GitClientVS.Infrastructure.Extensions;
using ParseDiff;

namespace GitClientVS.Services
{
    [Export(typeof(IGitClientService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class BitbucketService : IGitClientService
    {
        private readonly IEventAggregatorService _eventAggregator;
        private IBitbucketClient _bitbucketClient;

        public bool IsConnected => _bitbucketClient != null;

        [ImportingConstructor]
        public BitbucketService(IEventAggregatorService eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }


        public string Origin => "Bitbucket";
        public string Title => $"{Origin} Extension";
        private readonly string supportedSCM = "git";

        public async Task LoginAsync(GitCredentials gitCredentials)
        {
            if (IsConnected)
                return;

            if (string.IsNullOrEmpty(gitCredentials.Login) ||
                string.IsNullOrEmpty(gitCredentials.Password))
                throw new Exception("Credentials fields cannot be empty");

            _bitbucketClient = await CreateBitbucketClient(gitCredentials);

            var connectionData = new ConnectionData()
            {
                IsLoggedIn = true,
                UserName = _bitbucketClient.ApiConnection.Credentials.Login,
                Password = gitCredentials.Password,
                Host = gitCredentials.Host,
                IsEnterprise = gitCredentials.IsEnterprise
            };

            OnConnectionChanged(connectionData);
        }

        private async Task<IBitbucketClient> CreateBitbucketClient(GitCredentials gitCredentials)
        {
            var bitbucketClientFactory = new BitbucketClientFactory();//todo inject?

            var credentials = new Credentials(gitCredentials.Login, gitCredentials.Password);

            if (!gitCredentials.IsEnterprise)
                return await bitbucketClientFactory.CreateStandardBitBucketClient(gitCredentials.Host, credentials);
            else
                return await bitbucketClientFactory.CreateEnterpriseBitBucketClient(gitCredentials.Host, credentials);
        }

        public async Task<IEnumerable<GitRemoteRepository>> GetUserRepositoriesAsync()
        {
            var repositories = await _bitbucketClient.RepositoriesClient.GetRepositories();
            return repositories.Values.Where(repo => repo.Scm == supportedSCM).MapTo<List<GitRemoteRepository>>();
        }

        public async Task<IEnumerable<GitRemoteRepository>> GetAllRepositories()
        {
            var allRepositories = new List<GitRemoteRepository>();

            var userRepositories = await _bitbucketClient.RepositoriesClient.GetRepositories();
            allRepositories.AddRange(userRepositories.Values.Where(repo => repo.Scm == supportedSCM).MapTo<List<GitRemoteRepository>>());

            var teams = await _bitbucketClient.TeamsClient.GetTeams();
            foreach (var team in teams.Values)
            {
                var teamRepositories = await _bitbucketClient.RepositoriesClient.GetRepositories(team.Username);
                allRepositories.AddRange(teamRepositories.Values.Where(repo => repo.Scm == supportedSCM).MapTo<List<GitRemoteRepository>>());
            }

            return allRepositories;
        }

        public async Task<IEnumerable<GitTeam>> GetTeams()
        {
            var teams = await _bitbucketClient.TeamsClient.GetTeams();
            return teams.Values.MapTo<List<GitTeam>>();
        }

        public async Task<GitPullRequest> GetPullRequest(string repositoryName, string ownerName, long id)
        {
            return (await _bitbucketClient.PullRequestsClient.GetPullRequest(repositoryName, ownerName, id)).MapTo<GitPullRequest>();
        }

        public async Task<IEnumerable<FileDiff>> GetPullRequestDiff(string repositoryName, long id)
        {
            return await _bitbucketClient.PullRequestsClient.GetPullRequestDiff(repositoryName, id);
        }

        public async Task<IEnumerable<FileDiff>> GetPullRequestDiff(string repositoryName, string ownerName, long id)
        {
            return await _bitbucketClient.PullRequestsClient.GetPullRequestDiff(repositoryName, ownerName, id);
        }

        public bool IsOriginRepo(GitRemoteRepository gitRemoteRepository)
        {
            if (gitRemoteRepository?.CloneUrl == null) return false;
            Uri uri = new Uri(gitRemoteRepository.CloneUrl);
            return _bitbucketClient.ApiConnection.ApiUrl.Host.Contains(uri.Host, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<GitRemoteRepository> CreateRepositoryAsync(GitRemoteRepository newRepository)
        {
            var repository = newRepository.MapTo<Repository>();
            var result = await _bitbucketClient.RepositoriesClient.CreateRepository(repository);
            return result.MapTo<GitRemoteRepository>();
        }


        public async Task<IEnumerable<GitPullRequest>> GetAllPullRequests(string repositoryName, string ownerName)
        {
            //todo put real repository name
            var pullRequests = await _bitbucketClient.PullRequestsClient.GetAllPullRequests(repositoryName, ownerName);
            return pullRequests.Values.MapTo<List<GitPullRequest>>();
        }

        public async Task<PageIterator<GitPullRequest>> GetPullRequests(string repositoryName, string ownerName, int limit = 20, int page = 1)
        {
            //todo put real repository name
            var pullRequests = await _bitbucketClient.PullRequestsClient.GetPullRequestsPage(repositoryName, ownerName, limit: limit, page: page);
            return pullRequests.MapTo<PageIterator<GitPullRequest>>();
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

        public async Task<IEnumerable<GitBranch>> GetBranches(string repoName, string owner)
        {
            var repositories = await _bitbucketClient.RepositoriesClient.GetBranches(owner, repoName);
            return repositories.Values.MapTo<List<GitBranch>>();
        }

        public async Task<IEnumerable<GitUser>> GetPullRequestsAuthors(string repositoryName, string ownerName)
        {
            var authors = await _bitbucketClient.PullRequestsClient.GetAuthors(repositoryName, ownerName);
            return authors.Values.MapTo<List<GitUser>>();
        }

        public async Task<bool> ApprovePullRequest(string repositoryName, string ownerName, long id)
        {
            var result = await _bitbucketClient.PullRequestsClient.ApprovePullRequest(repositoryName, ownerName, id);
            return (result != null && result.Approved);
        }

        public async Task DisapprovePullRequest(string repositoryName, string ownerName, long id)
        {
            await _bitbucketClient.PullRequestsClient.DisapprovePullRequest(repositoryName, ownerName, id);
        }

        public async Task CreatePullRequest(GitPullRequest gitPullRequest, string repositoryName, string owner)
        {
            await _bitbucketClient.PullRequestsClient.CreatePullRequest(gitPullRequest.MapTo<PullRequest>(), repositoryName, owner);
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

        public async Task<IEnumerable<GitComment>> GetPullRequestComments(string repositoryName, string ownerName, long id)
        {
            var commits = await _bitbucketClient.PullRequestsClient.GetPullRequestComments(repositoryName, ownerName, id);
            return commits.Values.MapTo<List<GitComment>>();
        }

    }
}
