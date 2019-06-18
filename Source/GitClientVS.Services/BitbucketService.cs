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
using GitClientVS.Contracts.Interfaces;

namespace GitClientVS.Services
{
    [Export(typeof(IGitClientService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class BitbucketService : IGitClientService
    {
        private readonly IEventAggregatorService _eventAggregator;
        private readonly IGitWatcher _gitWatcher;
        private readonly IBitbucketClientFactory _bitbucketClientFactory;
        public IBitbucketClient Client { get; private set; }

        public bool IsConnected => Client != null;
        public string GitClientType => Client?.BitBucketType.ToString();

        [ImportingConstructor]
        public BitbucketService(
            IEventAggregatorService eventAggregator, 
            IGitWatcher gitWatcher,
            IBitbucketClientFactory bitbucketClientFactory)
        {
            _eventAggregator = eventAggregator;
            _gitWatcher = gitWatcher;
            _bitbucketClientFactory = bitbucketClientFactory;
        }


        public string Origin => "Bitbucket";
        public string Title => $"{Origin} Extension";
        private readonly string supportedSCM = "git";

        public async Task LoginAsync(GitCredentials gitCredentials)
        {
            if (IsConnected)
                return;

            OnConnectionChanged(new ConnectionData() { IsLoggingIn = true });

            if (string.IsNullOrEmpty(gitCredentials.Login) ||
                string.IsNullOrEmpty(gitCredentials.Password))
                throw new Exception("Credentials fields cannot be empty");

            ConnectionData connectionData = ConnectionData.NotLogged;

            try
            {
                 connectionData = new ConnectionData()
                {
                    Password = gitCredentials.Password,
                    Host = gitCredentials.Host,
                    IsEnterprise = gitCredentials.IsEnterprise,
                    IsLoggingIn = false
                };
                Client = await CreateBitbucketClient(gitCredentials);

                connectionData.UserName = Client.ApiConnection.Credentials.Login;
                connectionData.Id = Client.ApiConnection.Credentials.AccountId;
                connectionData.IsLoggedIn = true;
            }
            finally
            {
                OnConnectionChanged(connectionData);
            }
        }

        private async Task<IBitbucketClient> CreateBitbucketClient(GitCredentials gitCredentials)
        {
            var credentials = new Credentials(gitCredentials.Login, gitCredentials.Password); //todo this is wrong for enterprise, should pass UIID

            if (!gitCredentials.IsEnterprise)
                return await _bitbucketClientFactory.CreateStandardBitBucketClient(credentials);
            else
                return await _bitbucketClientFactory.CreateEnterpriseBitBucketClient(gitCredentials.Host, credentials);
        }


        public async Task<IEnumerable<GitUser>> GetRepositoryUsers(string filter)
        {
            return (await Client.PullRequestsClient
                .GetRepositoryUsers(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, filter))
                .MapTo<List<GitUser>>();
        }

        public async Task<IEnumerable<GitRemoteRepository>> GetAllRepositories()
        {
            var userRepositories = (await Client.RepositoriesClient.GetUserRepositories())
                .Where(repo => repo.Scm == supportedSCM)
                .MapTo<List<GitRemoteRepository>>();

            return userRepositories;
        }

        public async Task<IEnumerable<GitTeam>> GetTeams()
        {
            var teams = await Client.TeamsClient.GetTeams();
            return teams.MapTo<List<GitTeam>>();
        }

        public async Task<GitPullRequest> GetPullRequest(long id)
        {
            return (await Client.PullRequestsClient
                .GetPullRequest(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, id))
                .MapTo<GitPullRequest>();
        }

        public async Task<IEnumerable<FileDiff>> GetPullRequestDiff(long id)
        {
            var diffs = (await Client
                .PullRequestsClient
                .GetPullRequestDiff(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, id)).ToList();

            ProcessDiffs(diffs);

            return diffs;
        }

        public async Task<IEnumerable<FileDiff>> GetCommitsDiff(string fromCommit, string toCommit)
        {
            var diffs = (await Client
                .PullRequestsClient
                .GetCommitsDiff(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, fromCommit, toCommit)).ToList();

            ProcessDiffs(diffs);

            return diffs;
        }



        public async Task<string> GetFileContent(string hash, string path)
        {
            return await Client.PullRequestsClient.GetFileContent(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, hash, path);
        }

        public async Task<GitComment> EditPullRequestComment(long id, GitComment comment)
        {
            var response = await Client.PullRequestsClient.EditPullRequestComment(
                _gitWatcher.ActiveRepo.Name,
                _gitWatcher.ActiveRepo.Owner,
                id,
                comment.Id,
                comment.Content.Html,
                comment.Version
                );

            return response.MapTo<GitComment>();
        }

        public async Task<GitComment> AddPullRequestComment(long id, GitComment comment)
        {
            var response = await Client.PullRequestsClient.AddPullRequestComment(
                _gitWatcher.ActiveRepo.Name,
                _gitWatcher.ActiveRepo.Owner,
                id,
                comment.Content.Html,
                comment.Inline?.From,
                comment.Inline?.To,
                comment.Inline?.Path,
                comment.Parent?.Id);

            return response.MapTo<GitComment>();
        }

        public async Task DeletePullRequestComment(long pullRequestId, long commentId, long version)
        {
            await Client.PullRequestsClient.DeletePullRequestComment(
                _gitWatcher.ActiveRepo.Name,
                _gitWatcher.ActiveRepo.Owner,
                pullRequestId,
                commentId,
                version
                );
        }

        public bool IsOriginRepo(GitRemoteRepository gitRemoteRepository)
        {
            if (gitRemoteRepository?.CloneUrl == null) return false;
            Uri uri = new Uri(gitRemoteRepository.CloneUrl);
            return Client.ApiConnection.ApiUrl.Host.Contains(uri.Host, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<GitRemoteRepository> CreateRepositoryAsync(GitRemoteRepository newRepository)
        {
            var repository = newRepository.MapTo<Repository>();
            repository.Name = repository.Name.Replace(' ', '-');
            var result = await Client.RepositoriesClient.CreateRepository(repository, newRepository.IsTeam);
            return result.MapTo<GitRemoteRepository>();
        }

        public async Task<GitPullRequest> GetPullRequestForBranches(string sourceBranch, string destBranch)
        {
            var pullRequest = await Client.PullRequestsClient
                .GetPullRequestForBranches(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, sourceBranch, destBranch);
            return pullRequest?.MapTo<GitPullRequest>();
        }


        public async Task<IEnumerable<GitCommit>> GetCommitsRange(GitBranch fromBranch, GitBranch toBranch)
        {
            var from = new Branch()
            {
                Name = fromBranch.Name,
                Target = new Commit() { Hash = fromBranch.Target.Hash }
            };

            var to = new Branch()
            {
                Name = toBranch.Name,
                Target = new Commit() { Hash = toBranch.Target.Hash }
            };

            var commits = await Client.RepositoriesClient.GetCommitsRange(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, from, to);
            return commits.MapTo<List<GitCommit>>();
        }

        public async Task<IEnumerable<GitPullRequest>> GetPullRequests(
            int limit = 50,
            GitPullRequestStatus? state = null,
            string fromBranch = null,
            string toBranch = null,
            bool isDescSorted = true,
            string author = null
            )
        {
            var builder = Client.PullRequestsClient.GetPullRequestQueryBuilder()
                .WithState(state?.ToString() ?? "ALL")
                .WithOrder(isDescSorted ? Order.Newest : Order.Oldest)
                .WithSourceBranch(fromBranch)
                .WithDestinationBranch(toBranch)
                .WithAuthor(author, null);

            return (await Client.PullRequestsClient
                .GetPullRequests(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, limit, builder))
                .MapTo<List<GitPullRequest>>();
        }

        public async Task<PageIterator<GitPullRequest>> GetPullRequestsPage(
           int page,
           int limit = 50,
           GitPullRequestStatus? state = null,
           string fromBranch = null,
           string toBranch = null,
           bool isDescSorted = true,
           string author = null
           )
        {
            var builder = Client.PullRequestsClient.GetPullRequestQueryBuilder()
                .WithState(state?.ToString() ?? "ALL")
                .WithOrder(isDescSorted ? Order.Newest : Order.Oldest)
                .WithSourceBranch(fromBranch)
                .WithDestinationBranch(toBranch)
                .WithAuthor(author, null);

            return (await Client.PullRequestsClient
                    .GetPullRequestsPage(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, page, limit, builder))
                    .MapTo<PageIterator<GitPullRequest>>();
        }

        public async Task<IEnumerable<GitBranch>> GetBranches()
        {
            var repositories = await Client.RepositoriesClient.GetBranches(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner);
            return repositories.MapTo<List<GitBranch>>();
        }

        public async Task<GitCommit> GetCommitById(string id)
        {
            var commit = await Client.RepositoriesClient.GetCommitById(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, id);
            return commit.MapTo<GitCommit>();
        }

        public async Task<IEnumerable<GitUser>> GetPullRequestsAuthors()
        {
            var authors = await Client.PullRequestsClient.GetAuthors(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner);
            return authors.MapTo<List<GitUser>>();
        }

        public async Task<bool> ApprovePullRequest(long id)
        {
            var result = await Client.PullRequestsClient.ApprovePullRequest(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, id);
            return (result != null && result.Approved);
        }

        public async Task<bool> DeclinePullRequest(long id, string version)
        {
            await Client.PullRequestsClient.DeclinePullRequest(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, id, version);
            return true;
        }

        public async Task<bool> MergePullRequest(GitMergeRequest request)
        {
            var req = request.MapTo<MergeRequest>();
            await Client.PullRequestsClient.MergePullRequest(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, req);
            return true;
        }

        public async Task DisapprovePullRequest(long id)
        {
            await Client.PullRequestsClient.DisapprovePullRequest(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, id);
        }

        public async Task CreatePullRequest(GitPullRequest gitPullRequest)
        {
            await Client.PullRequestsClient.CreatePullRequest(gitPullRequest.MapTo<PullRequest>(), _gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner);
        }

        public async Task UpdatePullRequest(GitPullRequest gitPullRequest)
        {
            await Client.PullRequestsClient
                .UpdatePullRequest(gitPullRequest.MapTo<PullRequest>(), _gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner);
        }

        public async Task<IEnumerable<GitUser>> GetDefaultReviewers()
        {
            return (await Client.PullRequestsClient
                .GetDefaultReviewers(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner))
                .MapTo<List<GitUser>>();
        }

        public void Logout()
        {
            Client = null;
            OnConnectionChanged(ConnectionData.NotLogged);
        }

        public async Task ChangeUserAsync(GitCredentials credentials)
        {
            Client = null;
            await LoginAsync(credentials);
        }

        private static void ProcessDiffs(IEnumerable<FileDiff> diffs)
        {
            //todo word level
        }

        private void OnConnectionChanged(ConnectionData connectionData)
        {
            _eventAggregator.Publish(new ConnectionChangedEvent(connectionData));
        }

        public async Task<IEnumerable<GitCommit>> GetPullRequestCommits(long id)
        {
            var commits = await Client.PullRequestsClient.GetPullRequestCommits(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, id);
            return commits.MapTo<List<GitCommit>>();
        }

        public async Task<IEnumerable<GitComment>> GetPullRequestComments(long id)
        {
            var comments = (await Client.PullRequestsClient.GetPullRequestComments(_gitWatcher.ActiveRepo.Name,
                _gitWatcher.ActiveRepo.Owner, id)).ToList();

            AssignInlinesToChildren(comments);

            return comments.MapTo<List<GitComment>>();
        }

        private static void AssignInlinesToChildren(List<Comment> comments)
        {
            var commentDictionary = comments.ToDictionary(x => x.Id.Value, x => x);

            foreach (var comment in comments.ToList())
            {
                var ancestors = new List<Comment>();

                Comment current = comment;
                do
                {
                    ancestors.Add(current);
                    current = GetParent(commentDictionary, current);
                }
                while (current != null);

                var firstAncestor = ancestors.Last();

                foreach (var ancestor in ancestors)
                    ancestor.Inline = firstAncestor.Inline;
            }
        }

        private static Comment GetParent(Dictionary<long, Comment> commentDictionary, Comment current)
        {
            return current.Parent != null ? commentDictionary[current.Parent.Id] : null;
        }
    }
}
