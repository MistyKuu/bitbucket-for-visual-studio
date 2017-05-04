using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models;
using ReactiveUI;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(IPullRequestsWindowContainerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PullRequestsWindowContainerViewModel : ViewModelBase, IPullRequestsWindowContainerViewModel
    {
        private readonly IPageNavigationService<IPullRequestsWindow> _pageNavigationService;
        private readonly IEventAggregatorService _eventAggregator;
        private readonly IGitService _gitService;
        private readonly ICacheService _cacheService;
        private readonly IUserInformationService _userInfoService;
        private IView _currentView;
        private ReactiveCommand<Unit,Unit> _prevCommand;
        private ReactiveCommand<Unit, Unit> _nextCommand;
        private IWithPageTitle _currentViewModel;
        private Theme _currentTheme;

        public ShowConfirmationEventViewModel ConfirmationViewModel { get; }
        public string ActiveRepository { get; set; }

        public IView CurrentView
        {
            get => _currentView;
            set => this.RaiseAndSetIfChanged(ref _currentView, value);
        }

        public IWithPageTitle CurrentViewModel
        {
            get => _currentViewModel;
            set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
        }

        public Theme CurrentTheme
        {
            get => _currentTheme;
            set => this.RaiseAndSetIfChanged(ref _currentTheme, value);
        }

        public ICommand PrevCommand => _prevCommand;
        public ICommand NextCommand => _nextCommand;

        [ImportingConstructor]
        public PullRequestsWindowContainerViewModel(
            IPageNavigationService<IPullRequestsWindow> pageNavigationService,
            IEventAggregatorService eventAggregator,
            IGitService gitService,
            ICacheService cacheService,
            IUserInformationService userInfoService
            )
        {
            _pageNavigationService = pageNavigationService;
            _eventAggregator = eventAggregator;
            _gitService = gitService;
            _cacheService = cacheService;
            _userInfoService = userInfoService;

            _prevCommand = ReactiveCommand.Create(() => _pageNavigationService.NavigateBack(), _pageNavigationService.CanNavigateBackObservable);
            _nextCommand = ReactiveCommand.Create(() => _pageNavigationService.NavigateForward(), _pageNavigationService.CanNavigateForwardObservable);

            var repo = _gitService.GetActiveRepository();
            ActiveRepository = repo.Owner + '/' + repo.Name;

            CurrentTheme = _userInfoService.CurrentTheme;
            ConfirmationViewModel = new ShowConfirmationEventViewModel();
        }

        protected override IEnumerable<IDisposable> SetupObservables()
        {
            yield return _pageNavigationService.Where(x => x.Window == typeof(IPullRequestsWindow)).Subscribe(ChangeView);
            yield return _eventAggregator.GetEvent<ThemeChangedEvent>().Subscribe(ev => CurrentTheme = ev.Theme);
            yield return _eventAggregator.GetEvent<ShowConfirmationEvent>().Subscribe(ShowConfirmation);
            yield return _eventAggregator
                .GetEvent<ActiveRepositoryChangedEvent>()
                .Where(x => x.IsRepositoryDifferent)
                .Select(x => Unit.Default)
                .Merge(_eventAggregator.GetEvent<ConnectionChangedEvent>().Select(x => Unit.Default))
                .Subscribe(_ => OnClosed());
            
            this.WhenAnyValue(x => x.CurrentView).Where(x => x != null).Subscribe(_ => CurrentViewModel = CurrentView.DataContext as IWithPageTitle);
            
            this.WhenAnyObservable(x => x._nextCommand, x => x._prevCommand)
                .Subscribe(_ => ConfirmationViewModel.Event = null);
        }

        private void ChangeView(NavigationEvent navEvent)
        {
            CurrentView = navEvent.View;
        }

        public event EventHandler Closed;

        protected virtual void OnClosed()
        {
            _cacheService.Delete(CacheKeys.PullRequestCacheKey);
            _pageNavigationService.ClearNavigationHistory();
            Closed?.Invoke(this, EventArgs.Empty);
            Dispose();
        }

        private void ShowConfirmation(ShowConfirmationEvent ev)
        {
            ConfirmationViewModel.Event = ev;
        }
    }
}
