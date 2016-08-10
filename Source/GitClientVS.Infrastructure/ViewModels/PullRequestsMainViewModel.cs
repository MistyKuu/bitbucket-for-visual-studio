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
        private IEnumerable<GitPullRequest> _gitPullRequests;
        private List<GitPullRequest> _filteredGitPullRequests;
        private string _errorMessage;
        private ReactiveCommand<object> _goToCreateNewPullRequestCommand;

        private List<GitUser> _authors;
        private GitUser _selectedAuthor;
        private GitPullRequestStatus _selectedStatus;

        public IEnumerable<GitPullRequest> GitPullRequests
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

        public List<GitPullRequest> FilteredGitPullRequests
        {
            get { return _filteredGitPullRequests; }
            set { this.RaiseAndSetIfChanged(ref _filteredGitPullRequests, value); }
        }

        public List<GitUser> Authors
        {
            get { return _authors; }
            set { this.RaiseAndSetIfChanged(ref _authors, value); }
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
                return new SuggestionProvider(x => Authors.Where(
                    y => y.DisplayName.Contains(x, StringComparison.InvariantCultureIgnoreCase) || y.Username.Contains(x, StringComparison.InvariantCultureIgnoreCase)));
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
            _filteredGitPullRequests = new List<GitPullRequest>();
            SetupObservables();

            SelectedStatus = GitPullRequestStatus.Open;
            Authors = new List<GitUser>();
        }

        private void SetupObservables()
        {
            this.WhenAnyValue(x => x.SelectedStatus).Subscribe(_ => Filter());
            this.WhenAnyValue(x => x.SelectedAuthor).Subscribe(_ => Filter());
            this.WhenAnyValue(x => x.GitPullRequests).Where(x => x != null).Subscribe(_ => Authors = GitPullRequests.Select(x => x.Author).ToList());
            _initializeCommand.Subscribe(_ => { Filter(); });
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
            GitPullRequests = await _gitClientService.GetPullRequests("django-piston", "jespern");
        }

        private bool CanRunFilter()
        {
            return GitPullRequests != null;
        }

        private void Filter()
        {
            if (!CanRunFilter())
                return;

            FilteredGitPullRequests = GitPullRequests
                .Where(pullRequest => pullRequest.Status == SelectedStatus)
                .Where(pullRequest => SelectedAuthor == null || pullRequest.Author.Username == SelectedAuthor.Username)
                .ToList();
        }

        private IObservable<bool> CanLoadPullRequests()
        {
            return Observable.Return(true);
        }
    }
}
