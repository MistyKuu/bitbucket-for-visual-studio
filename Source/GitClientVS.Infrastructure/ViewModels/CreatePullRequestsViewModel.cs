using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
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
        private bool _isLoading;
        private string _errorMessage;
        private ReactiveCommand<Unit> _createNewPullRequestCommand;
        private IEnumerable<GitBranch> _remoteBranches;
        private IEnumerable<GitBranch> _localBranches;
        private GitBranch _sourceBranch;
        private GitBranch _destinationBranch;
        private string _description;
        private string _Title;
        private bool _closeSourceBranch;
        private string _message;

        public string PageTitle { get; } = "Create New Pull Request";

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { this.RaiseAndSetIfChanged(ref _errorMessage, value); }
        }

        public IEnumerable<GitBranch> LocalBranches
        {
            get { return _localBranches; }
            set { this.RaiseAndSetIfChanged(ref _localBranches, value); }
        }

        public IEnumerable<GitBranch> RemoteBranches
        {
            get { return _remoteBranches; }
            set { this.RaiseAndSetIfChanged(ref _remoteBranches, value); }
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
                .SelectMany(async _ => await _initializeCommand.ExecuteAsync())
                .Subscribe();

            this.WhenAnyValue(x => x.SourceBranch)
                .Where(x => x != null)
                .Subscribe(_ =>
                {
                    var currentBranch = RemoteBranches.FirstOrDefault(x => SourceBranch.Name == x.Name);
                    if (currentBranch == null)
                        Message = $"Warning! Selected branch {SourceBranch.Name} is not a remote branch.";
                    else if (currentBranch.Target.Hash != SourceBranch.Target.Hash)
                        Message = $"Warning! Selected branch {SourceBranch.Name} is out of sync with a remote branch.";
                    else
                        Message = string.Empty;
                });
        }

        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateAsyncTask(CanLoadPullRequests(), _ => LoadBranches());
            _createNewPullRequestCommand = ReactiveCommand.CreateAsyncTask(CanCreatePullRequest(), _ => CreateNewPullRequest());

            _createNewPullRequestCommand.Subscribe(_ =>
            {
                _pageNavigationService.NavigateBack(true);
            });
        }


        private async Task CreateNewPullRequest()
        {
            var currentRepo = _gitService.GetActiveRepository();
            var gitPullRequest = new GitPullRequest(Title, Description, SourceBranch.Name, DestinationBranch.Name)
            {
                CloseSourceBranch = CloseSourceBranch
            };
            await _gitClientService.CreatePullRequest(gitPullRequest, currentRepo.Name, currentRepo.Owner);
        }

        private async Task LoadBranches()
        {
            var activeRepo = _gitService.GetActiveRepository();

            LocalBranches = activeRepo.Branches.Where(x => !x.IsRemote).OrderBy(x => x.Name).ToList();
            RemoteBranches = (await _gitClientService.GetBranches(activeRepo.Name, activeRepo.Owner)).OrderBy(x => x.Name).ToList();

            SourceBranch = LocalBranches.FirstOrDefault(x => x.IsHead);
            DestinationBranch = RemoteBranches.FirstOrDefault(x => x.Name != SourceBranch?.Name);
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
    }
}
