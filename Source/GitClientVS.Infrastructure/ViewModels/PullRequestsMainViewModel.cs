using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using GitClientVS.Infrastructure.Utils;
using ReactiveUI;
using WpfControls;
using SuggestionProvider = WpfControls.SuggestionProvider;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(IPullRequestsMainViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PullRequestsMainViewModel : ViewModelBase, IPullRequestsMainViewModel
    {
        private readonly IGitClientService _gitClientService;
        private readonly IGitService _gitService;
        private readonly IPageNavigationService _pageNavigationService;
        private ReactiveCommand<Unit> _initializeCommand;
        private ReactiveCommand<object> _goToDetailsCommand;
        private bool _isLoading;
        private ReactiveList<GitPullRequest> _gitPullRequests;
        private ReactiveList<GitPullRequest> _filteredGitPullRequests;
        private string _errorMessage;
        private ReactiveCommand<object> _goToCreateNewPullRequestCommand;

        private List<GitUser> _authors;
        private GitUser _selectedAuthor;
        private GitPullRequestStatus _selectedStatus;
        private GitPullRequest _selectedPullRequest;

        public ReactiveList<GitPullRequest> GitPullRequests
        {
            get { return _gitPullRequests; }
            set { this.RaiseAndSetIfChanged(ref _gitPullRequests, value); }
        }

        public GitUser SelectedAuthor
        {
            get { return _selectedAuthor; }
            set { this.RaiseAndSetIfChanged(ref _selectedAuthor, value); }
        }

        public GitPullRequestStatus SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedStatus, value);
            }
        }

        public ReactiveList<GitPullRequest> FilteredGitPullRequests
        {
            get { return _filteredGitPullRequests; }
            set { this.RaiseAndSetIfChanged(ref _filteredGitPullRequests, value); }
        }

        public List<GitUser> Authors
        {
            get { return _authors; }
            set { this.RaiseAndSetIfChanged(ref _authors, value); }
        }

        public GitPullRequest SelectedPullRequest
        {
            get { return _selectedPullRequest; }
            set { this.RaiseAndSetIfChanged(ref _selectedPullRequest, value); }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { this.RaiseAndSetIfChanged(ref _errorMessage, value); }
        }

        public IEnumerable<IReactiveCommand> ThrowableCommands => new[] { _initializeCommand };
        public IEnumerable<IReactiveCommand> LoadingCommands => new[] { _initializeCommand };

        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }

        public ICommand InitializeCommand => _initializeCommand;
        public ICommand GoToDetailsCommand => _goToDetailsCommand;
        public ICommand GotoCreateNewPullRequestCommand => _goToCreateNewPullRequestCommand;

        public ISuggestionProvider AuthorProvider
        {
            get
            {
                return new SuggestionProvider(x => Authors.Where(y =>
                (y.DisplayName != null && y.DisplayName.Contains(x, StringComparison.InvariantCultureIgnoreCase)) ||
                (y.Username != null && y.Username.Contains(x, StringComparison.InvariantCultureIgnoreCase))));
            }
        }




        [ImportingConstructor]
        public PullRequestsMainViewModel(
            IGitClientService gitClientService,
            IGitService gitService,
            IPageNavigationService pageNavigationService
            )
        {
            _gitClientService = gitClientService;
            _gitService = gitService;
            _pageNavigationService = pageNavigationService;
            GitPullRequests = new ReactiveList<GitPullRequest>();
            FilteredGitPullRequests = new ReactiveList<GitPullRequest>();
            SetupObservables();

            SelectedStatus = GitPullRequestStatus.Open;
            Authors = new List<GitUser>();
        }

        private void SetupObservables()
        {
            this.WhenAny(x => x.SelectedStatus, x => SelectedAuthor).Subscribe(_ => Filter());
            _initializeCommand.Subscribe(_ => { Filter(); });
            this.WhenAnyValue(x => x.GitPullRequests).Where(x => x != null).Subscribe(_ => Authors = GitPullRequests.Select(x => x.Author).ToList());
            this.WhenAnyValue(x => x.SelectedPullRequest).Where(x => x != null).Subscribe(_ => _goToDetailsCommand.Execute(SelectedPullRequest));
        }

        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateAsyncTask(CanLoadPullRequests(), _ => LoadPullRequests());
            _goToCreateNewPullRequestCommand = ReactiveCommand.Create(Observable.Return(true));
            _goToCreateNewPullRequestCommand.Subscribe(_ => { _pageNavigationService.Navigate(PageIds.CreatePullRequestsPageId); });

            _goToDetailsCommand = ReactiveCommand.Create(Observable.Return(true));
            _goToDetailsCommand.Subscribe(x => { _pageNavigationService.Navigate(PageIds.PullRequestsDetailPageId, x); });
        }

        private async Task LoadPullRequests()
        {
            var requests = await _gitClientService.GetPullRequests("atlassian-rest", "atlassian");
            GitPullRequests = new ReactiveList<GitPullRequest>(requests);
        }

        private bool CanRunFilter()
        {
            return GitPullRequests != null;
        }

        private void Filter()
        {
            if (!CanRunFilter())
                return;

            FilteredGitPullRequests = new ReactiveList<GitPullRequest>(GitPullRequests
                .Where(pullRequest => pullRequest.Status == SelectedStatus)
                .Where(pullRequest => SelectedAuthor == null || pullRequest.Author.Username == SelectedAuthor.Username));
        }

        private IObservable<bool> CanLoadPullRequests()
        {
            return this.WhenAnyValue(x => x.IsLoading).Select(x => !IsLoading);
        }
    }
}
