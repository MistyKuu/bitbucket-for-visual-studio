using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GitClientVS.Contracts;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
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
        private readonly IPageNavigationService<IPullRequestsWindow> _pageNavigationService;
        private readonly ICacheService _cacheService;
        private ReactiveCommand<Unit> _initializeCommand;
        private ReactiveCommand<object> _goToDetailsCommand;
        private ReactiveCommand<Unit> _loadNextPageCommand;
        private bool _isLoading;
        private string _errorMessage;
        private ReactiveCommand<object> _goToCreateNewPullRequestCommand;
        private ReactiveCommand<Unit> _refreshPullRequestsCommand;

        private List<GitUser> _authors;
        private GitUser _selectedAuthor;
        private GitPullRequestStatus? _selectedStatus;
        private GitPullRequest _selectedPullRequest;
        private GitRemoteRepository _currentRepository;
        private PagedCollection<GitPullRequest> _gitPullRequests;

        public GitUser SelectedAuthor
        {
            get { return _selectedAuthor; }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedAuthor, value);
            }
        }

        public GitPullRequestStatus? SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedStatus, value);
            }
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

        public IEnumerable<IReactiveCommand> ThrowableCommands => new[] { _initializeCommand, _loadNextPageCommand, _refreshPullRequestsCommand };
        public IEnumerable<IReactiveCommand> LoadingCommands => new[] { _initializeCommand, _loadNextPageCommand, _refreshPullRequestsCommand };

        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }

        public PagedCollection<GitPullRequest> GitPullRequests
        {
            get { return _gitPullRequests; }
            set { this.RaiseAndSetIfChanged(ref _gitPullRequests, value); }
        }

        public string PageTitle { get; } = "Pull Requests";

        public ICommand InitializeCommand => _initializeCommand;
        public ICommand GoToDetailsCommand => _goToDetailsCommand;
        public ICommand GotoCreateNewPullRequestCommand => _goToCreateNewPullRequestCommand;
        public ICommand LoadNextPageCommand => _loadNextPageCommand;
        public ICommand RefreshPullRequestsCommand => _refreshPullRequestsCommand;

        public ISuggestionProvider AuthorProvider
        {
            get
            {
                return new SuggestionProvider(x => Authors.Where(y =>
                (y.DisplayName != null && y.DisplayName.Contains(x, StringComparison.InvariantCultureIgnoreCase)) ||
                (y.Username != null && y.Username.Contains(x, StringComparison.InvariantCultureIgnoreCase))));
            }
        }

        private const int PageSize = 50;
        private bool _isInitialized = false;

        [ImportingConstructor]
        public PullRequestsMainViewModel(
            IGitClientService gitClientService,
            IGitService gitService,
            IPageNavigationService<IPullRequestsWindow> pageNavigationService,
            ICacheService cacheService
            )
        {
            _gitClientService = gitClientService;
            _gitService = gitService;
            _pageNavigationService = pageNavigationService;
            _cacheService = cacheService;
            _currentRepository = _gitService.GetActiveRepository();
            SelectedStatus = GitPullRequestStatus.Open;
            Authors = new List<GitUser>();
        }

        protected override IEnumerable<IDisposable> SetupObservables()
        {
            this.WhenAnyValue(x => x.SelectedStatus, x => x.SelectedAuthor)
                .Select(x => new { SelectedStatus, SelectedAuthor })
                .DistinctUntilChanged()
                .Where(x => _isInitialized)
                .Where(x => !IsLoading)
                .Subscribe(_ => { _refreshPullRequestsCommand.Execute(null); });

            this.WhenAnyValue(x => x.SelectedPullRequest).Where(x => x != null).Subscribe(_ => _goToDetailsCommand.Execute(SelectedPullRequest));

            _goToDetailsCommand.Subscribe(x => { _pageNavigationService.Navigate<IPullRequestDetailView>(((GitPullRequest)x).Id); });
            _goToCreateNewPullRequestCommand.Subscribe(_ => { _pageNavigationService.Navigate<ICreatePullRequestsView>(); });

            yield break;
        }

        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateAsyncTask(CanLoadPullRequests(), async _ =>
            {
                Authors = (await _gitClientService.GetPullRequestsAuthors()).ToList();
                await RefreshPullRequests();
                _isInitialized = true;
            });
            _goToCreateNewPullRequestCommand = ReactiveCommand.Create(Observable.Return(true));
            _goToDetailsCommand = ReactiveCommand.Create(Observable.Return(true));
            _loadNextPageCommand = ReactiveCommand.CreateAsyncTask(Observable.Return(true), _ => GitPullRequests.LoadNextPageAsync());
            _refreshPullRequestsCommand = ReactiveCommand.CreateAsyncTask(Observable.Return(true), _ => RefreshPullRequests());
        }

        private async Task RefreshPullRequests()
        {
            GitPullRequests = new PagedCollection<GitPullRequest>(GetPullRequestsPage, PageSize);
            await GitPullRequests.LoadNextPageAsync();
        }

        private async Task<IEnumerable<GitPullRequest>> GetPullRequestsPage(int pageSize, int page)
        {
            var iterator = await _gitClientService.GetPullRequestsPage(
                state: SelectedStatus,
                author: SelectedAuthor?.Username,
                limit: pageSize,
                page: page
                );

            return iterator.Values;
        }

        private IObservable<bool> CanLoadPullRequests()
        {
            return this.WhenAnyValue(x => x.IsLoading).Select(x => !IsLoading);
        }
    }
}
