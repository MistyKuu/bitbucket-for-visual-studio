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
        private ReactiveCommand<Unit> _initializeCommand;
        private bool _isLoading;
        private string _errorMessage;
        private ReactiveCommand<Unit> _createNewPullRequestCommand;
        private IEnumerable<GitBranch> _branches;
        private GitBranch _sourceBranch;
        private GitBranch _destinationBranch;
        private string _description;
        private string _Title;
        private bool _closeSourceBranch;
        private bool _isSync;

        public string PageTitle { get; } = "Create New Pull Request";

        public bool IsSync
        {
            get { return _isSync; }
            set { this.RaiseAndSetIfChanged(ref _isSync, value); }
        }

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

        [ValidatesViaMethod(AllowBlanks = false, AllowNull = false, Name = nameof(ValidateDestinationBranch), ErrorMessage = "Branches must be different")]
        public GitBranch DestinationBranch
        {
            get { return _destinationBranch; }
            set { this.RaiseAndSetIfChanged(ref _destinationBranch, value); }
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

        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }

        public ICommand InitializeCommand => _initializeCommand;
        public ICommand CreateNewPullRequestCommand => _createNewPullRequestCommand;

        [ImportingConstructor]
        public CreatePullRequestsViewModel(
            IGitClientServiceFactory gitClientServiceFactory,
            IGitService gitService,
            IPageNavigationService<IPullRequestsWindow> pageNavigationService
            )
        {
            _gitClientService = gitClientServiceFactory.GetService();
            _gitService = gitService;
            _pageNavigationService = pageNavigationService;
            CloseSourceBranch = false;
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
            await _gitClientService.CreatePullRequest(gitPullRequest,currentRepo);
        }

        private async Task LoadBranches()
        {
            var activeRepo = _gitService.GetActiveRepository();
            var activeBranch = _gitService.GetActiveBranchFromActiveRepository();
            var lastCommit = _gitService.GetHeadCommitOfActiveBranch();

            Branches = (await _gitClientService.GetBranches(activeRepo)).ToList();
            SourceBranch = Branches.FirstOrDefault(x => x.Name.Equals(activeBranch, StringComparison.InvariantCultureIgnoreCase));

            if (SourceBranch != null)
            {
                IsSync = SourceBranch.Target.Hash == lastCommit;
            }


            DestinationBranch = Branches.FirstOrDefault();
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
                   !string.IsNullOrEmpty(SourceBranch.Name) &&
                   !string.IsNullOrEmpty(DestinationBranch.Name);

        }

        public bool ValidateDestinationBranch(GitBranch destBranch)
        {
            return destBranch.Name != SourceBranch.Name;
        }
    }
}
