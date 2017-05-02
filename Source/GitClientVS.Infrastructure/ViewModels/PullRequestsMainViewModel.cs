using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GitClientVS.Contracts;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
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
        private readonly IPageNavigationService<IPullRequestsWindow> _pageNavigationService;
        private ReactiveCommand _initializeCommand;
        private ReactiveCommand _goToDetailsCommand;
        private ReactiveCommand _loadNextPageCommand;
        private ReactiveCommand _goToCreateNewPullRequestCommand;
        private ReactiveCommand _refreshPullRequestsCommand;
        private bool _isLoading;
        private string _errorMessage;

        private List<GitUser> _authors;
        private GitUser _selectedAuthor;
        private GitPullRequestStatus? _selectedStatus;
        private GitPullRequest _selectedPullRequest;
        private PagedCollection<GitPullRequest> _gitPullRequests;

        public GitUser SelectedAuthor
        {
            get => _selectedAuthor;
            set => this.RaiseAndSetIfChanged(ref _selectedAuthor, value);
        }

        public GitPullRequestStatus? SelectedStatus
        {
            get => _selectedStatus;
            set => this.RaiseAndSetIfChanged(ref _selectedStatus, value);
        }

        public List<GitUser> Authors
        {
            get => _authors;
            set => this.RaiseAndSetIfChanged(ref _authors, value);
        }

        public GitPullRequest SelectedPullRequest
        {
            get => _selectedPullRequest;
            set => this.RaiseAndSetIfChanged(ref _selectedPullRequest, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public IEnumerable<ReactiveCommand> ThrowableCommands => new[] { _initializeCommand, _loadNextPageCommand, _refreshPullRequestsCommand };
        public IEnumerable<ReactiveCommand> LoadingCommands => new[] { _initializeCommand, _loadNextPageCommand, _refreshPullRequestsCommand };

        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        public PagedCollection<GitPullRequest> GitPullRequests
        {
            get => _gitPullRequests;
            set => this.RaiseAndSetIfChanged(ref _gitPullRequests, value);
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
            IPageNavigationService<IPullRequestsWindow> pageNavigationService
            )
        {
            _gitClientService = gitClientService;
            _pageNavigationService = pageNavigationService;
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
                .Select(x => Unit.Default)
                .InvokeCommand(_refreshPullRequestsCommand);

            this.WhenAnyValue(x => x.SelectedPullRequest)
                .Where(x => x != null)
                .InvokeCommand(_goToDetailsCommand);

            yield break;
        }

        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateFromTask(async _ =>
            {
                if (_isInitialized)
                    return;

                await RefreshPullRequests();
                _isInitialized = true;
            }, CanLoadPullRequests());

            _goToCreateNewPullRequestCommand = ReactiveCommand.Create(() => { _pageNavigationService.Navigate<ICreatePullRequestsView>(); });
            _goToDetailsCommand = ReactiveCommand.Create<GitPullRequest>(x => _pageNavigationService.Navigate<IPullRequestDetailView>(x.Id));
            _loadNextPageCommand = ReactiveCommand.CreateFromTask(_ => GitPullRequests.LoadNextPageAsync());
            _refreshPullRequestsCommand = ReactiveCommand.CreateFromTask(_ => RefreshPullRequests());
        }

        private async Task RefreshPullRequests()
        {
            Authors = (await _gitClientService.GetPullRequestsAuthors()).ToList();
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
