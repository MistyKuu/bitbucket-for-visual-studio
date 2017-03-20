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
        private readonly IUserInformationService _userInformationService;
        private readonly ITreeStructureGenerator _treeStructureGenerator;
        private string _errorMessage;
        private bool _isLoading;
        private ReactiveCommand<Unit> _initializeCommand;
        private ReactiveCommand<Unit> _approveCommand;
        private GitPullRequest _pullRequest;
        private string _mainSectionCommandText;
        private bool _isApproveAvailable;
        private bool _isApproved;
        private Theme _currentTheme;
        private List<GitUser> _selectedReviewers;


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

        public PullRequestDiffModel PullRequestDiffModel { get; set; }


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
            _userInformationService = userInformationService;
            _treeStructureGenerator = treeStructureGenerator;
            CurrentTheme = userInformationService.CurrentTheme;
            PullRequestDiffModel = new PullRequestDiffModel(commandsService);

            eventAggregatorService.GetEvent<ThemeChangedEvent>().Subscribe(ev =>
            {
                CurrentTheme = ev.Theme;
                PullRequestDiffModel.CommentTree = _treeStructureGenerator.CreateCommentTree(PullRequestDiffModel.Comments.ToList(), CurrentTheme).ToList();
            });
            this.WhenAnyValue(x => x.PullRequest).Subscribe(_ => this.RaisePropertyChanged(nameof(PageTitle)));
        }

        public ICommand InitializeCommand => _initializeCommand;
        public ICommand ApproveCommand => _approveCommand;

        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateAsyncTask(Observable.Return(true), x => LoadPullRequestData((long)x));
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
            PullRequestDiffModel.Comments = (await _gitClientService.GetPullRequestComments(id)).Where(comment => comment.IsFile == false).ToList();
            PullRequestDiffModel.CommentTree = _treeStructureGenerator.CreateCommentTree(PullRequestDiffModel.Comments.ToList(), CurrentTheme).ToList();
        }

        private async Task CreateCommits(long id)
        {
            PullRequestDiffModel.Commits = (await _gitClientService.GetPullRequestCommits(id)).ToList();
        }

        private async Task CreateDiffContent(long id)
        {
            var fileDiffs = (await _gitClientService.GetPullRequestDiff(id)).ToList();
            PullRequestDiffModel.FilesTree = _treeStructureGenerator.CreateFileTree(fileDiffs).ToList();
            PullRequestDiffModel.FileDiffs = fileDiffs;
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

        public IEnumerable<IReactiveCommand> ThrowableCommands => new[] { _initializeCommand };
        public IEnumerable<IReactiveCommand> LoadingCommands => new[] { _initializeCommand };

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
