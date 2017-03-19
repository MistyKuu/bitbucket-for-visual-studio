using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BitBucket.REST.API.Models.Standard;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using log4net;
using ParseDiff;
using ReactiveUI;
using WpfControls;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(ICreatePullRequestsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CreatePullRequestsViewModel : ViewModelBase, ICreatePullRequestsViewModel
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IGitClientService _gitClientService;
        private readonly IGitService _gitService;
        private readonly IPageNavigationService<IPullRequestsWindow> _pageNavigationService;
        private readonly IEventAggregatorService _eventAggregator;
        private readonly ITreeStructureGenerator _treeStructureGenerator;
        private ReactiveCommand<Unit> _initializeCommand;
        private ReactiveCommand<object> _removeReviewerCommand;
        private bool _isLoading;
        private string _errorMessage;
        private ReactiveCommand<Unit> _createNewPullRequestCommand;
        private IEnumerable<GitBranch> _branches;
        private GitBranch _sourceBranch;
        private GitBranch _destinationBranch;
        private string _description;
        private string _title;
        private bool _closeSourceBranch;
        private string _message;
        private GitRemoteRepository _currentRepo;
        private ReactiveList<GitUser> _selectedReviewers;
        private GitPullRequest _remotePullRequest;
        private ReactiveCommand<object> _goToDetailsCommand;

        public string PageTitle { get; } = "Create New Pull Request";


        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { this.RaiseAndSetIfChanged(ref _errorMessage, value); }
        }

        public IEnumerable<GitBranch> Branches
        {
            get { return _branches; }
            set { this.RaiseAndSetIfChanged(ref _branches, value); }
        }


        [Required]

        public GitBranch SourceBranch
        {
            get { return _sourceBranch; }
            set { this.RaiseAndSetIfChanged(ref _sourceBranch, value); }
        }


        [Required]
        public GitBranch DestinationBranch
        {
            get { return _destinationBranch; }
            set { this.RaiseAndSetIfChanged(ref _destinationBranch, value); }
        }

        public string Message
        {
            get { return _message; }
            set { this.RaiseAndSetIfChanged(ref _message, value); }
        }

        public string Description
        {
            get { return _description; }
            set { this.RaiseAndSetIfChanged(ref _description, value); }
        }

        [Required]
        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }

        [Required]
        public bool CloseSourceBranch
        {
            get { return _closeSourceBranch; }
            set { this.RaiseAndSetIfChanged(ref _closeSourceBranch, value); }
        }

        public PullRequestDiffModel PullRequestDiffModel { get; set; }

        public string ExistingBranchText => RemotePullRequest == null ? null : $"#{RemotePullRequest.Id} {RemotePullRequest.Title} (created {RemotePullRequest.Created})";

        public IEnumerable<IReactiveCommand> ThrowableCommands => new[] { _initializeCommand, _createNewPullRequestCommand };

        public IEnumerable<IReactiveCommand> LoadingCommands => new[] { _initializeCommand, _createNewPullRequestCommand };

        public string GitClientType => _gitClientService.GitClientType;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }

        public ICommand InitializeCommand => _initializeCommand;
        public ICommand CreateNewPullRequestCommand => _createNewPullRequestCommand;
        public ICommand RemoveReviewerCommand => _removeReviewerCommand;
        public ICommand GoToDetailsCommand => _goToDetailsCommand;

        [ImportingConstructor]
        public CreatePullRequestsViewModel(
            IGitClientService gitClientService,
            IGitService gitService,
            IPageNavigationService<IPullRequestsWindow> pageNavigationService,
            IEventAggregatorService eventAggregator,
            ITreeStructureGenerator treeStructureGenerator,
            ICommandsService commandsService
        )
        {
            _gitClientService = gitClientService;
            _gitService = gitService;
            _pageNavigationService = pageNavigationService;
            _eventAggregator = eventAggregator;
            _treeStructureGenerator = treeStructureGenerator;
            PullRequestDiffModel = new PullRequestDiffModel(commandsService);

            CloseSourceBranch = false;
            SelectedReviewers = new ReactiveList<GitUser>();
            SetupObservables();
        }


        private void SetupObservables()
        {
            _eventAggregator.GetEvent<ActiveRepositoryChangedEvent>()
                .SelectMany(_ => _initializeCommand.ExecuteAsyncTask())
                .Subscribe();

            this.WhenAnyValue(x => x.SourceBranch, x => x.DestinationBranch)
                .Where((x, y) => x.Item1 != null && x.Item2 != null)
                .SelectMany(_ => CheckPullRequestExistence().ToObservable())
                .Subscribe();
        }

        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateAsyncTask(CanLoadPullRequests(), _ => LoadBranches());

            _removeReviewerCommand = ReactiveCommand.Create();
            _createNewPullRequestCommand = ReactiveCommand.CreateAsyncTask(CanCreatePullRequest(), _ => CreateNewPullRequest());

            _createNewPullRequestCommand.Subscribe(_ => { _pageNavigationService.NavigateBack(true); });
            _removeReviewerCommand.Subscribe((x) => { SelectedReviewers.Remove((GitUser)x); });

            _goToDetailsCommand = ReactiveCommand.Create(Observable.Return(true));
            _goToDetailsCommand.Subscribe(id => { _pageNavigationService.Navigate<IPullRequestDetailView>(id); });
        }


        private async Task CreateNewPullRequest()
        {
            var gitPullRequest = new GitPullRequest(Title, Description, SourceBranch.Name, DestinationBranch.Name)
            {
                CloseSourceBranch = CloseSourceBranch,
                Reviewers = SelectedReviewers.ToDictionary(x => x, x => true)
            };
            await _gitClientService.CreatePullRequest(gitPullRequest);
        }

        private async Task LoadBranches()
        {
            _currentRepo = _gitService.GetActiveRepository();
            var currentBranch = _currentRepo.Branches.First(x => x.IsHead);

            Branches = (await _gitClientService.GetBranches()).OrderBy(x => x.Name).ToList();

            SourceBranch = Branches.FirstOrDefault(x => x.Name == currentBranch.TrackedBranchName) ??
                           Branches.FirstOrDefault();
            DestinationBranch = Branches.FirstOrDefault(x => x.IsDefault) ??
                                Branches.FirstOrDefault(x => x.Name != SourceBranch.Name);

            Message = string.IsNullOrEmpty(currentBranch.TrackedBranchName)
                ? $"Warning! Your active local branch {currentBranch.Name} is not tracking any remote branches."
                : string.Empty;
        }

        private async Task CheckPullRequestExistence()
        {
            var pullRequest = await _gitClientService.GetPullRequestForBranches(SourceBranch.Name, DestinationBranch.Name);
            PullRequestDiffModel.Commits = (await _gitClientService.GetCommitsRange(SourceBranch, DestinationBranch)).ToList();

            await CreateDiffContent(SourceBranch.Target.Hash, DestinationBranch.Target.Hash);

            if (pullRequest != null)
            {
                Title = pullRequest.Title;
                Description = pullRequest.Description;
                SelectedReviewers.Clear();
                foreach (var reviewer in pullRequest.Reviewers.Select(x => x.Key))
                    SelectedReviewers.Add(reviewer);
            }
            else
            {
                SetPullRequestDataFromCommits(PullRequestDiffModel.Commits);
            }

            RemotePullRequest = pullRequest;
            this.RaisePropertyChanged(nameof(ExistingBranchText));
        }


        private IObservable<bool> CanLoadPullRequests()
        {
            return Observable.Return(true);
        }

        private IObservable<bool> CanCreatePullRequest()
        {
            return ValidationObservable.Select(x => CanExecute()).StartWith(CanExecute());
        }

        private bool CanExecute()
        {
            return IsObjectValid() &&
                   !string.IsNullOrEmpty(SourceBranch?.Name) &&
                   !string.IsNullOrEmpty(DestinationBranch?.Name) &&
                   ValidateBranches();
        }

        public bool ValidateBranches()
        {
            return DestinationBranch?.Name != SourceBranch?.Name;
        }

        public ReactiveList<GitUser> SelectedReviewers
        {
            get { return _selectedReviewers; }
            set { this.RaiseAndSetIfChanged(ref _selectedReviewers, value); }
        }

        public GitPullRequest RemotePullRequest
        {
            get { return _remotePullRequest; }
            set { this.RaiseAndSetIfChanged(ref _remotePullRequest, value); }
        }

        public ISuggestionProvider ReviewersProvider => new SuggestionProvider(Filter);

        private IEnumerable Filter(string arg)
        {
            if (arg.Length < 3)
                return Enumerable.Empty<GitUser>();

            try
            {
                var suggestions = _gitClientService.GetRepositoryUsers(arg).Result;
                return suggestions.Except(SelectedReviewers, x => x.Username).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Enumerable.Empty<GitUser>();
            }
        }
        private void SetPullRequestDataFromCommits(List<GitCommit> commits)
        {
            if (commits.Count == 1)
            {
                Title = commits.First().Message.Trim();
                Description = string.Empty;
            }
            else
            {
                Title = SourceBranch.Name;
                Description = string.Join(Environment.NewLine, commits.Select((x) => $"* " + x.Message.Trim()).Reverse());
            }
        }

        private async Task CreateDiffContent(string fromCommit, string toCommit)
        {
            var fileDiffs = (await _gitClientService.GetCommitsDiff(fromCommit, toCommit)).ToList();
            PullRequestDiffModel.FilesTree = _treeStructureGenerator.CreateFileTree(fileDiffs).ToList();
            PullRequestDiffModel.FileDiffs = fileDiffs;
        }
    }
}
