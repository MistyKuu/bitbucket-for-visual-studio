using System;
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
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using ReactiveUI;
using WpfControls;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(ICreatePullRequestsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CreatePullRequestsViewModel : ViewModelBase, ICreatePullRequestsViewModel
    {
        private readonly IGitClientService _gitClientService;
        private readonly IGitService _gitService;
        private readonly IPageNavigationService<IPullRequestsWindow> _pageNavigationService;
        private readonly IEventAggregatorService _eventAggregator;
        private ReactiveCommand<Unit> _initializeCommand;
        private ReactiveCommand<object> _removeReviewerCommand;
        private bool _isLoading;
        private string _errorMessage;
        private ReactiveCommand<Unit> _createNewPullRequestCommand;
        private IEnumerable<GitBranch> _branches;
        private GitBranch _sourceBranch;
        private GitBranch _destinationBranch;
        private string _description;
        private string _Title;
        private bool _closeSourceBranch;
        private string _message;
        private GitRemoteRepository _currentRepo;
        private ReactiveList<GitUser> _selectedReviewers;
        private List<GitUser> _reviewers;
        private List<GitUser> _allReviewers;

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
            get { return _Title; }
            set { this.RaiseAndSetIfChanged(ref _Title, value); }
        }

        [Required]
        public bool CloseSourceBranch
        {
            get { return _closeSourceBranch; }
            set { this.RaiseAndSetIfChanged(ref _closeSourceBranch, value); }
        }

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

        [ImportingConstructor]
        public CreatePullRequestsViewModel(
            IGitClientService gitClientService,
            IGitService gitService,
            IPageNavigationService<IPullRequestsWindow> pageNavigationService,
            IEventAggregatorService eventAggregator
            )
        {
            _gitClientService = gitClientService;
            _gitService = gitService;
            _pageNavigationService = pageNavigationService;
            _eventAggregator = eventAggregator;
            CloseSourceBranch = false;
            SetupObservables();
        }

        private void SetupObservables()
        {
            _eventAggregator.GetEvent<ActiveRepositoryChangedEvent>()
                .SelectMany(_ => LoadBranches().ToObservable())
                .Subscribe();

            this.WhenAnyObservable(x => x.SelectedReviewers.Changed).Subscribe(_ =>
            {
                Reviewers = AllReviewers.Except(SelectedReviewers).ToList();
            });
        }

        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateAsyncTask(CanLoadPullRequests(), async _ =>
            {
                await LoadBranches();
                await SetReviewers();
            });

            _removeReviewerCommand = ReactiveCommand.Create();
            _createNewPullRequestCommand = ReactiveCommand.CreateAsyncTask(CanCreatePullRequest(), _ => CreateNewPullRequest());

            _createNewPullRequestCommand.Subscribe(_ => { _pageNavigationService.NavigateBack(true); });
            _removeReviewerCommand.Subscribe((x) => { SelectedReviewers.Remove((GitUser)x); });
        }


        private async Task CreateNewPullRequest()
        {
            var gitPullRequest = new GitPullRequest(Title, Description, SourceBranch.Name, DestinationBranch.Name)
            {
                CloseSourceBranch = CloseSourceBranch,
                Reviewers = SelectedReviewers.ToDictionary(x => x.Username, x => true)
            };
            await _gitClientService.CreatePullRequest(gitPullRequest, _currentRepo.Name, _currentRepo.Owner);
        }

        private async Task LoadBranches()
        {
            _currentRepo = _gitService.GetActiveRepository();
            var currentBranch = _currentRepo.Branches.First(x => x.IsHead);

            Branches = (await _gitClientService.GetBranches(_currentRepo.Name, _currentRepo.Owner)).OrderBy(x => x.Name).ToList();

            SourceBranch = Branches.FirstOrDefault(x => x.Name == currentBranch.TrackedBranchName) ?? Branches.FirstOrDefault();
            DestinationBranch = Branches.FirstOrDefault(x => x.IsDefault) ??
                                Branches.FirstOrDefault(x => x.Name != SourceBranch.Name);

            Message = string.IsNullOrEmpty(currentBranch.TrackedBranchName) ?
                $"Warning! Your active local branch {currentBranch.Name} is not tracking any remote branches." :
                string.Empty;
        }

        private async Task SetReviewers()
        {
            AllReviewers = new List<GitUser>(await _gitClientService.GetRepositoryUsers(_currentRepo.Name, _currentRepo.Owner));
            Reviewers = AllReviewers.ToList();
            SelectedReviewers = new ReactiveList<GitUser>();
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
            get
            {
                return _selectedReviewers;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedReviewers, value);
            }
        }

        public List<GitUser> Reviewers
        {
            get
            {
                return _reviewers;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _reviewers, value);
            }
        }

        public List<GitUser> AllReviewers
        {
            get
            {
                return _allReviewers;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _allReviewers, value);
            }
        }

        public ISuggestionProvider ReviewersProvider
        {
            get
            {
                return new SuggestionProvider(x => Reviewers.Where(y =>
                (y.DisplayName != null && y.DisplayName.Contains(x, StringComparison.InvariantCultureIgnoreCase)) ||
                (y.Username != null && y.Username.Contains(x, StringComparison.InvariantCultureIgnoreCase))));
            }
        }
    }
}
