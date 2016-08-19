using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.Views;
using ReactiveUI;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(IPullRequestsWindowContainerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PullRequestsWindowContainerViewModel : ViewModelBase, IPullRequestsWindowContainerViewModel
    {
        private readonly IPageNavigationService<IPullRequestsWindow> _pageNavigationService;
        private IView _currentView;
        private readonly ReactiveCommand<object> _prevCommand;
        private readonly ReactiveCommand<object> _nextCommand;
        private IWithPageTitle _currentViewModel;

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

        public ICommand PrevCommand => _prevCommand;
        public ICommand NextCommand => _nextCommand;

        [ImportingConstructor]
        public PullRequestsWindowContainerViewModel(
            IPullRequestsMainView pullRequestStartView,
            IPageNavigationService<IPullRequestsWindow> pageNavigationService
            )
        {
            _pageNavigationService = pageNavigationService;
            _pageNavigationService.Where(x => x.Window == typeof(IPullRequestsWindow)).Subscribe(ChangeView);
            _prevCommand = ReactiveCommand.Create(_pageNavigationService.CanNavigateBackObservable);
            _prevCommand.Subscribe(_ => _pageNavigationService.NavigateBack());
            _nextCommand = ReactiveCommand.Create(_pageNavigationService.CanNavigateForwardObservable);
            _nextCommand.Subscribe(_ => _pageNavigationService.NavigateForward());
            this.WhenAnyValue(x => x.CurrentView).Where(x => x != null).Subscribe(_ => CurrentViewModel = CurrentView.DataContext as IWithPageTitle);
        }


        private void ChangeView(NavigationEvent navEvent)
        {
            CurrentView = navEvent.View;
        }
    }
}
