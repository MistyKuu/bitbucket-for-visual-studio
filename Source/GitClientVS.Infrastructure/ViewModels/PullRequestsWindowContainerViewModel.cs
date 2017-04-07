using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly ReactiveCommand<object> _prevCommand;
        private readonly ReactiveCommand<object> _nextCommand;
        private IWithPageTitle _currentViewModel;
        private Theme _currentTheme;

        public ShowConfirmationEventViewModel ConfirmationViewModel { get; }
        public string ActiveRepository { get; set; }

        public IView CurrentView
        {
            get { return _currentView; }
            set
            {
                this.RaiseAndSetIfChanged(ref _currentView, value);
            }
        }

        public IWithPageTitle CurrentViewModel
        {
            get { return _currentViewModel; }
            set { this.RaiseAndSetIfChanged(ref _currentViewModel, value); }
        }

        public Theme CurrentTheme
        {
            get { return _currentTheme; }
            set
            {
                this.RaiseAndSetIfChanged(ref _currentTheme, value);
            }
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
            _pageNavigationService.Where(x => x.Window == typeof(IPullRequestsWindow)).Subscribe(ChangeView);
            _prevCommand = ReactiveCommand.Create(_pageNavigationService.CanNavigateBackObservable);
            _prevCommand.Subscribe(_ => _pageNavigationService.NavigateBack());
            _nextCommand = ReactiveCommand.Create(_pageNavigationService.CanNavigateForwardObservable);
            _nextCommand.Subscribe(_ => _pageNavigationService.NavigateForward());
            this.WhenAnyValue(x => x.CurrentView).Where(x => x != null).Subscribe(_ => CurrentViewModel = CurrentView.DataContext as IWithPageTitle);
            _eventAggregator
                .GetEvent<ActiveRepositoryChangedEvent>()
                .Where(x => x.IsRepositoryDifferent)
                .Select(x => Unit.Default)
                .Merge(_eventAggregator.GetEvent<ConnectionChangedEvent>().Select(x => Unit.Default))
                .Subscribe(_ => OnClosed());

            var repo = _gitService.GetActiveRepository();
            ActiveRepository = repo.Owner + '/' + repo.Name;
            _eventAggregator.GetEvent<ThemeChangedEvent>().Subscribe(ev => CurrentTheme = ev.Theme);
            _eventAggregator.GetEvent<ShowConfirmationEvent>().Subscribe(ShowConfirmation);

            CurrentTheme = _userInfoService.CurrentTheme;
            ConfirmationViewModel = new ShowConfirmationEventViewModel();

            this.WhenAnyObservable(x => x._nextCommand, x => x._prevCommand).Subscribe(_ => ConfirmationViewModel.Event = null);
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
        }

        private void ShowConfirmation(ShowConfirmationEvent ev)
        {
            ConfirmationViewModel.Event = ev;
        }
    }
}
