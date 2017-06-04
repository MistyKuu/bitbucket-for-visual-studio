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
using DiffPlex;
using DiffPlex.DiffBuilder;
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
        private readonly IGitWatcher _gitWatcher;
        private IBitbucketClient _bitbucketClient;

        public bool IsConnected => _bitbucketClient != null;
        public string GitClientType => _bitbucketClient?.BitBucketType.ToString();

        [ImportingConstructor]
        public BitbucketService(IEventAggregatorService eventAggregator, IGitWatcher gitWatcher)
        {
            _eventAggregator = eventAggregator;
            _gitWatcher = gitWatcher;
        }


        public string Origin => "Bitbucket";
        public string Title => $"{Origin} Extension";
        private readonly string supportedSCM = "git";
        private static readonly SideBySideDiffBuilder DiffBuilder = new SideBySideDiffBuilder(new Differ());

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
                return await bitbucketClientFactory.CreateStandardBitBucketClient(credentials);
            else
                return await bitbucketClientFactory.CreateEnterpriseBitBucketClient(gitCredentials.Host, credentials);
        }


        public async Task<IEnumerable<GitUser>> GetRepositoryUsers(string filter)
        {
            return (await _bitbucketClient.PullRequestsClient
                .GetRepositoryUsers(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, filter))
                .MapTo<List<GitUser>>();
        }

        public async Task<IEnumerable<GitRemoteRepository>> GetAllRepositories()
        {
            var userRepositories = (await _bitbucketClient.RepositoriesClient.GetUserRepositories())
                .Where(repo => repo.Scm == supportedSCM)
                .MapTo<List<GitRemoteRepository>>();

            return userRepositories;
        }

        public async Task<IEnumerable<GitTeam>> GetTeams()
        {
            var teams = await _bitbucketClient.TeamsClient.GetTeams();
            return teams.MapTo<List<GitTeam>>();
        }

        public async Task<GitPullRequest> GetPullRequest(long id)
        {
            return (await _bitbucketClient.PullRequestsClient
                .GetPullRequest(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, id))
                .MapTo<GitPullRequest>();
        }

        public async Task<IEnumerable<FileDiff>> GetPullRequestDiff(long id)
        {
            var diffs =(await _bitbucketClient
                .PullRequestsClient
                .GetPullRequestDiff(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, id)).ToList();

            ProcessDiffs(diffs);

            return diffs;
        }

        public async Task<IEnumerable<FileDiff>> GetCommitsDiff(string fromCommit, string toCommit)
        {
            var diffs= (await  _bitbucketClient
                .PullRequestsClient
                .GetCommitsDiff(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, fromCommit, toCommit)).ToList();

            ProcessDiffs(diffs);

            return diffs;
        }

       

        public async Task<string> GetFileContent(string hash, string path)
        {
            return await _bitbucketClient.PullRequestsClient.GetFileContent(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, hash, path);
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
            repository.Name = repository.Name.Replace(' ', '-');
            var result = await _bitbucketClient.RepositoriesClient.CreateRepository(repository);
            return result.MapTo<GitRemoteRepository>();
        }

        public async Task<GitPullRequest> GetPullRequestForBranches(string sourceBranch, string destBranch)
        {
            var pullRequest = await _bitbucketClient.PullRequestsClient
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

            var commits = await _bitbucketClient.RepositoriesClient.GetCommitsRange(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, from, to);
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
            var builder = _bitbucketClient.PullRequestsClient.GetPullRequestQueryBuilder()
                .WithState(state?.ToString() ?? "ALL")
                .WithOrder(isDescSorted ? Order.Newest : Order.Oldest)
                .WithSourceBranch(fromBranch)
                .WithDestinationBranch(toBranch)
                .WithAuthor(author, null);

            return (await _bitbucketClient.PullRequestsClient
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
            var builder = _bitbucketClient.PullRequestsClient.GetPullRequestQueryBuilder()
                .WithState(state?.ToString() ?? "ALL")
                .WithOrder(isDescSorted ? Order.Newest : Order.Oldest)
                .WithSourceBranch(fromBranch)
                .WithDestinationBranch(toBranch)
                .WithAuthor(author, null);

            return (await _bitbucketClient.PullRequestsClient
                    .GetPullRequestsPage(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, page, limit, builder))
                    .MapTo<PageIterator<GitPullRequest>>();
        }

        public async Task<IEnumerable<GitBranch>> GetBranches()
        {
            var repositories = await _bitbucketClient.RepositoriesClient.GetBranches(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner);
            return repositories.MapTo<List<GitBranch>>();
        }

        public async Task<GitCommit> GetCommitById(string id)
        {
            var commit = await _bitbucketClient.RepositoriesClient.GetCommitById(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, id);
            return commit.MapTo<GitCommit>();
        }

        public async Task<IEnumerable<GitUser>> GetPullRequestsAuthors()
        {
            var authors = await _bitbucketClient.PullRequestsClient.GetAuthors(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner);
            return authors.MapTo<List<GitUser>>();
        }

        public async Task<bool> ApprovePullRequest(long id)
        {
            var result = await _bitbucketClient.PullRequestsClient.ApprovePullRequest(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, id);
            return (result != null && result.Approved);
        }

        public async Task<bool> DeclinePullRequest(long id, string version)
        {
            await _bitbucketClient.PullRequestsClient.DeclinePullRequest(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, id, version);
            return true;
        }

        public async Task<bool> MergePullRequest(GitMergeRequest request)
        {
            var req = request.MapTo<MergeRequest>();
            await _bitbucketClient.PullRequestsClient.MergePullRequest(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, req);
            return true;
        }

        public async Task DisapprovePullRequest(long id)
        {
            await _bitbucketClient.PullRequestsClient.DisapprovePullRequest(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, id);
        }

        public async Task CreatePullRequest(GitPullRequest gitPullRequest)
        {
            await _bitbucketClient.PullRequestsClient.CreatePullRequest(gitPullRequest.MapTo<PullRequest>(), _gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner);
        }

        public async Task UpdatePullRequest(GitPullRequest gitPullRequest)
        {
            await _bitbucketClient.PullRequestsClient
                .UpdatePullRequest(gitPullRequest.MapTo<PullRequest>(), _gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner);
        }

        public async Task<IEnumerable<GitUser>> GetDefaultReviewers()
        {
            return (await _bitbucketClient.PullRequestsClient
                .GetDefaultReviewers(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner))
                .MapTo<List<GitUser>>();
        }

        public void Logout()
        {
            _bitbucketClient = null;
            OnConnectionChanged(ConnectionData.NotLogged);
        }

        private static void ProcessDiffs(IEnumerable<FileDiff> diffs)
        {
            foreach (var chunk in diffs.SelectMany(x => x.Chunks))
            {
                var pairs = from added in chunk.Changes.Where(x => x.Type == LineChangeType.Add)
                    join deleted in chunk.Changes.Where(x => x.Type == LineChangeType.Delete)
                    on added.NewIndex equals deleted.OldIndex
                    select new { Added = added, Deleted = deleted };

                foreach (var pair in pairs)
                {
                    var result = DiffBuilder.BuildDiffModel(pair.Deleted.Content, pair.Added.Content);
                    pair.Deleted.ChangesInLine = result.OldText; //todo REWRITE EVERYTHING TO USE THIS LIB
                    pair.Added.ChangesInLine = result.NewText;
                }
            }
        }

        private void OnConnectionChanged(ConnectionData connectionData)
        {
            _eventAggregator.Publish(new ConnectionChangedEvent(connectionData));
        }

        public async Task<IEnumerable<GitCommit>> GetPullRequestCommits(long id)
        {
            var commits = await _bitbucketClient.PullRequestsClient.GetPullRequestCommits(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, id);
            return commits.MapTo<List<GitCommit>>();
        }

        public async Task<IEnumerable<GitComment>> GetPullRequestComments(long id)
        {
            var commits = await _bitbucketClient.PullRequestsClient.GetPullRequestComments(_gitWatcher.ActiveRepo.Name, _gitWatcher.ActiveRepo.Owner, id);
            return commits.MapTo<List<GitComment>>();
        }
    }
}
