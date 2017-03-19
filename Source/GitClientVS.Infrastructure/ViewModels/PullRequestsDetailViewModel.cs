using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using ParseDiff;
using ReactiveUI;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(IPullRequestsDetailViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PullRequestsDetailViewModel : ViewModelBase, IPullRequestsDetailViewModel
    {
        private readonly IGitClientService _gitClientService;
        private readonly IGitService _gitService;
        private readonly ICommandsService _commandsService;
        private readonly IUserInformationService _userInformationService;
        private readonly ITreeStructureGenerator _treeStructureGenerator;
        private readonly IVsTools _vsTools;
        private string _errorMessage;
        private bool _isLoading;
        private ReactiveCommand<Unit> _initializeCommand;
        private ReactiveCommand<Unit> _approveCommand;
        private IEnumerable<GitCommit> _commits;
        private IEnumerable<GitComment> _comments;
        private ReactiveCommand<Unit> _showDiffCommand;
        private IEnumerable<FileDiff> _fileDiffs;
        private List<ITreeFile> _filesTree;
        private List<ICommentTree> _commentTree;
        private GitPullRequest _pullRequest;
        private string _mainSectionCommandText;
        private bool _isApproveAvailable;
        private bool _isApproved;
        private Theme _currentTheme;


        public string PageTitle => "Pull Request Details";

        public bool IsApproveAvailable
        {
            get { return _isApproveAvailable; }
            set { this.RaiseAndSetIfChanged(ref _isApproveAvailable, value); }
        }

        public bool IsApproved
        {
            get { return _isApproved; }
            set { this.RaiseAndSetIfChanged(ref _isApproved, value); }
        }

        public string MainSectionCommandText
        {
            get { return _mainSectionCommandText; }
            set { this.RaiseAndSetIfChanged(ref _mainSectionCommandText, value); }
        }

        public IEnumerable<GitComment> Comments
        {
            get { return _comments; }
            set { this.RaiseAndSetIfChanged(ref _comments, value); }
        }

        public IEnumerable<GitCommit> Commits
        {
            get { return _commits; }
            set { this.RaiseAndSetIfChanged(ref _commits, value); }
        }

        public IEnumerable<FileDiff> FileDiffs
        {
            get { return _fileDiffs; }
            set { this.RaiseAndSetIfChanged(ref _fileDiffs, value); }
        }

        public List<ICommentTree> CommentTree
        {
            get { return _commentTree; }
            set { this.RaiseAndSetIfChanged(ref _commentTree, value); }
        }

        public List<ITreeFile> FilesTree
        {
            get { return _filesTree; }
            set { this.RaiseAndSetIfChanged(ref _filesTree, value); }
        }

        public GitPullRequest PullRequest
        {
            get { return _pullRequest; }
            set { this.RaiseAndSetIfChanged(ref _pullRequest, value); }
        }

        public Theme CurrentTheme
        {
            get { return _currentTheme; }
            set { this.RaiseAndSetIfChanged(ref _currentTheme, value); }
        }


        [ImportingConstructor]
        public PullRequestsDetailViewModel(
            IGitClientService gitClientService,
            IGitService gitService,
            ICommandsService commandsService,
            IUserInformationService userInformationService,
            IEventAggregatorService eventAggregatorService,
            ITreeStructureGenerator treeStructureGenerator
            )
        {
            _gitClientService = gitClientService;
            _gitService = gitService;
            _commandsService = commandsService;
            _userInformationService = userInformationService;
            _treeStructureGenerator = treeStructureGenerator;
            CurrentTheme = userInformationService.CurrentTheme;
            eventAggregatorService.GetEvent<ThemeChangedEvent>().Subscribe(ev =>
            {
                CurrentTheme = ev.Theme;
                CommentTree = _treeStructureGenerator.CreateCommentTree(Comments.ToList(), CurrentTheme).ToList();
            });
            this.WhenAnyValue(x => x._pullRequest).Subscribe(_ => this.RaisePropertyChanged(nameof(PageTitle)));
        }

        public ICommand InitializeCommand => _initializeCommand;
        public ICommand ShowDiffCommand => _showDiffCommand;
        public ICommand ApproveCommand => _approveCommand;

        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateAsyncTask(Observable.Return(true), x => LoadPullRequestData((long)x));
            _showDiffCommand = ReactiveCommand.CreateAsyncTask(Observable.Return(true), (x) => ShowDiff((FileDiff)x));
            _approveCommand = ReactiveCommand.CreateAsyncTask(Observable.Return(true), _ => Approve());
        }

        private async Task Approve()
        {
            if (IsApproved)
                await _gitClientService.DisapprovePullRequest(PullRequest.Id);
            else
                await _gitClientService.ApprovePullRequest(PullRequest.Id);

            // no exception means we did it!
            IsApproved = !IsApproved;

        }

        private Task ShowDiff(FileDiff diff)
        {
            _commandsService.ShowDiffWindow(diff, diff.Id);
            return Task.CompletedTask;
        }


        private async Task LoadPullRequestData(long id)
        {
            var tasks = new[]
            {
                GetPullRequestInfo(id),
                CreateCommits(id),
                CreateComments(id),
                CreateDiffContent(id)
            };

            await Task.WhenAll(tasks);
        }

        private async Task GetPullRequestInfo(long id)
        {
            PullRequest = await _gitClientService.GetPullRequest(id);
            CheckReviewers();
        }

        private async Task CreateComments(long id)
        {
            Comments = (await _gitClientService.GetPullRequestComments(id)).Where(comment => comment.IsFile == false);
            CommentTree = _treeStructureGenerator.CreateCommentTree(Comments.ToList(), CurrentTheme).ToList();
        }

        private async Task CreateCommits(long id)
        {
            Commits = await _gitClientService.GetPullRequestCommits(id);
        }

        private async Task CreateDiffContent(long id)
        {
            var fileDiffs = (await _gitClientService.GetPullRequestDiff(id)).ToList();
            FilesTree = _treeStructureGenerator.CreateFileTree(fileDiffs).ToList();
            FileDiffs = fileDiffs;
        }

        private void CheckReviewers()
        {
            IsApproved = true;
            foreach (var Reviewer in PullRequest.Reviewers)
            {
                if (Reviewer.Key.Username == _userInformationService.ConnectionData.UserName)
                {
                    IsApproveAvailable = true;
                    IsApproved = Reviewer.Value;
                }
            }
        }

        public IEnumerable<IReactiveCommand> ThrowableCommands => new[] { _initializeCommand, _showDiffCommand };
        public IEnumerable<IReactiveCommand> LoadingCommands => new[] { _initializeCommand, _showDiffCommand };

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                this.RaiseAndSetIfChanged(ref _errorMessage, value);
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }
    }
}
