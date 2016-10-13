using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure;
using GitClientVS.Infrastructure.Extensions;
using GitLab.NET;
using GitLab.NET.ResponseModels;

namespace GitClientVS.Services
{
    [Export(typeof(IGitClientService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [ExportMetadata(Consts.GitProviderKey, GitProviderType.Gitlab)]
    public class GitlabService : IGitClientService
    {
        private readonly IEventAggregatorService _eventAggregatorService;
        private GitLabClient _client;
        private User _userInfo;
        private const string HostUri = "https://gitlab.com/api/v3";


        public string Origin => "Gitlab";
        public string Title => $"{Origin} Extension";


        [ImportingConstructor]
        public GitlabService(IEventAggregatorService eventAggregatorService)
        {
            _eventAggregatorService = eventAggregatorService;
        }

        private void CheckStatusCode(HttpStatusCode statusCode)
        {
            //if (statusCode != HttpStatusCode.OK)
            //    throw new Exception();
        }

        public async Task LoginAsync(string login, string password)
        {
            _client = new GitLabClient(new Uri(HostUri));
            var session = await _client.Session.RequestSession(login, password);
            CheckStatusCode(session.StatusCode);
            _userInfo = session.Data;
            _client.PrivateToken = _userInfo.PrivateToken;
            OnConnectionChanged(ConnectionData.Create(_userInfo.Username, password, HostUri, GitProviderType.Gitlab));
        }

        public void Logout()
        {
            _client = null;
            OnConnectionChanged(ConnectionData.NotLogged);
        }

        public async Task<IEnumerable<GitRepository>> GetAllRepositories()
        {
            var result = await _client.Projects.Accessible();
            return result.Data.MapTo<List<GitRepository>>();
        }

        public async Task<GitRepository> CreateRepositoryAsync(GitRepository newRepository)
        {
            var ns = (await _client.Namespaces.GetAll()).Data.First(x => x.Path == newRepository.Owner);

            var project = await _client.Projects.Create(
                name: newRepository.Name,
                namespaceId: ns.Id,
                description: newRepository.Description,
                visibilityLevel: newRepository.IsPrivate ?? false ? VisibilityLevel.Private : VisibilityLevel.Public);

            return project.Data.MapTo<GitRepository>();
        }

        public async Task<IEnumerable<GitBranch>> GetBranches(GitRepository repo)
        {
            var project = await GetCurrentProject(repo);
            var result = await _client.Branches.GetAll(project.Id);
            return result.Data.MapTo<List<GitBranch>>();
        }

        public async Task<IEnumerable<GitTeam>> GetTeams()
        {
            var namespaces = await _client.Namespaces.GetAll();
            return namespaces.Data.MapTo<List<GitTeam>>();
        }


        public async Task<IEnumerable<GitCommit>> GetPullRequestCommits(GitRepository repo, long id)
        {
            var project = await GetCurrentProject(repo);
            var commits = await _client.MergeRequests.GetCommits(project.Id, (uint)id);

            var mapped = commits.Data.MapTo<List<GitCommit>>();
            return mapped;
        }

        public async Task<IEnumerable<GitComment>> GetPullRequestComments(GitRepository repo, long id)
        {
            var project = await GetCurrentProject(repo);
            var comments = await _client.MergeRequests.GetNotes(project.Id, (uint)id);
            return comments.Data.MapTo<List<GitComment>>();
        }

        public async Task<PageIterator<GitPullRequest>> GetPullRequests(GitRepository repo, int limit = 20, int page = 1)
        {
            var project = await GetCurrentProject(repo);
            var result = await _client.MergeRequests.GetAll(project.Id, null, null, null, (uint)page, (uint)limit);
            return result.MapTo<PageIterator<GitPullRequest>>();
        }

        public async Task<string> GetPullRequestDiff(GitRepository repo, long id)
        {
            var project = await GetCurrentProject(repo);
            var result = await _client.MergeRequests.GetChanges(project.Id, (uint)id);

            return string.Join(Environment.NewLine, result.Data.Changes.Select(x => x.Diff));
        }



        public async Task DisapprovePullRequest(GitRepository repo, long id)
        {
            throw new NotSupportedException("There is no disapprove in gitlab?");
        }

        public async Task<bool> ApprovePullRequest(GitRepository repo, long id)
        {
            var project = await GetCurrentProject(repo);
            var result = await _client.MergeRequests.Accept(project.Id, (uint)id);
            return result.Data != null;
        }

        public async Task<IEnumerable<GitUser>> GetPullRequestsAuthors(GitRepository repo)
        {
            var project = await GetCurrentProject(repo);
            var result = await _client.MergeRequests.GetAll(project.Id);
            return result.Data.Select(x => x.Author).DistinctBy(x => x.Name).MapTo<List<GitUser>>();
        }

        public bool IsOriginRepo(GitRepository gitRepository)
        {
            if (gitRepository?.CloneUrl == null) return false;
            Uri uri = new Uri(gitRepository.CloneUrl);
            return uri.Host.Contains("gitlab", StringComparison.OrdinalIgnoreCase);//TODO gitlab
        }

        public async Task<GitPullRequest> CreatePullRequest(GitPullRequest gitPullRequest, GitRepository repo)
        {
            var project = await GetCurrentProject(repo);
            var result = await _client.MergeRequests.Create(
                project.Id,
                gitPullRequest.SourceBranch,
                gitPullRequest.DestinationBranch,
                gitPullRequest.Title,
                gitPullRequest.Description);

            return result.Data.MapTo<GitPullRequest>();
        }

        public async Task<GitPullRequest> GetPullRequest(GitRepository repo, long id)
        {
            var project = await GetCurrentProject(repo);
            var mergeRequests = await _client.MergeRequests.Find(project.Id, (uint)id);
            return mergeRequests.Data.MapTo<GitPullRequest>();
        }

        private void OnConnectionChanged(ConnectionData connectionData)
        {
            _eventAggregatorService.Publish(new ConnectionChangedEvent(connectionData));
        }

        private async Task<Project> GetCurrentProject(GitRepository repo)
        {
            var projects = await _client.Projects.Accessible();
            var project = projects.Data.First(x => x.HttpUrlToRepo == repo.CloneUrl);
            return project;
        }
    }
}
