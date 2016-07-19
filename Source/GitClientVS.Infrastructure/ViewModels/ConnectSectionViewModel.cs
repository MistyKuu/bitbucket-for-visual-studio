using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces;
using ReactiveUI;
using System.Reactive.Linq;
using System.Windows.Input;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models;
using GitClientVS.Infrastructure.Events;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(IConnectSectionViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ConnectSectionViewModel : ViewModelBase, IConnectSectionViewModel
    {
        private readonly ExportFactory<ILoginDialogView> _loginViewFactory;
        private readonly IEventAggregatorService _eventAggregator;
        private readonly IGitClientService _gitClientService;
        private readonly ReactiveCommand<object> _openLoginCommand;
        private readonly ReactiveCommand<object> _logoutCommand;
        private readonly ReactiveCommand<Unit> _getRepositoriesCommand;

        private IDisposable _observable;
        private ConnectionData _connectionData;

        public ICommand OpenLoginCommand => _openLoginCommand;
        public ICommand LogoutCommand => _logoutCommand;
        public ICommand GetRepositoriesCommand => _getRepositoriesCommand;

        [ImportingConstructor]
        public ConnectSectionViewModel(
            ExportFactory<ILoginDialogView> loginViewFactory,
            IEventAggregatorService eventAggregator,
            IGitClientService gitClientService)
        {
            _loginViewFactory = loginViewFactory;
            _eventAggregator = eventAggregator;
            _gitClientService = gitClientService;

            _openLoginCommand = ReactiveCommand.Create(CanExecuteOpenLogin());
            _logoutCommand = ReactiveCommand.Create();
            _getRepositoriesCommand = ReactiveCommand.CreateAsyncTask(CanExecuteOpenLogin(), _ => GetRepositories());

            SetupObservables();
        }

        private async Task GetRepositories()
        {
            var repos = await _gitClientService.GetRepositoryAsync();
        }

        private void SetupObservables()
        {
            _openLoginCommand.Subscribe(_ => _loginViewFactory.CreateExport().Value.ShowModal());
            _logoutCommand.Subscribe(_ => { _gitClientService.Logout(); });

            _observable = _eventAggregator.GetEvent<ConnectionChangedEvent>().Subscribe(ConnectionChanged);
        }

        private void ConnectionChanged(ConnectionChangedEvent connectionChangedEvent)
        {
            ConnectionData = connectionChangedEvent.Data;
        }

        public ConnectionData ConnectionData
        {
            get { return _connectionData; }
            set { this.RaiseAndSetIfChanged(ref _connectionData, value); }
        }

        private IObservable<bool> CanExecuteOpenLogin()
        {
            return Observable.Return(true);
        }

        public void Dispose()
        {
            _observable.Dispose();
        }
    }
}
