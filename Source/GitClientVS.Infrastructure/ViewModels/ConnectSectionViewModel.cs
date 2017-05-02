using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using GitClientVS.Contracts.Interfaces;
using ReactiveUI;
using System.Windows.Input;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(IConnectSectionViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConnectSectionViewModel : ViewModelBase, IConnectSectionViewModel,IViewModelWithCommands
    {
        private readonly ExportFactory<ILoginDialogView> _loginViewFactory;
        private readonly ExportFactory<ICloneRepositoriesDialogView> _cloneRepoViewFactory;
        private readonly ExportFactory<ICreateRepositoriesDialogView> _createRepoViewFactory;
        private readonly IEventAggregatorService _eventAggregator;
        private readonly IUserInformationService _userInformationService;
        private readonly IGitClientService _gitClientService;
        private ReactiveCommand _openLoginCommand;
        private ReactiveCommand _logoutCommand;
        private ReactiveCommand _openCloneCommand;
        private ReactiveCommand _openCreateCommand;
        private ConnectionData _connectionData;

        public ICommand OpenLoginCommand => _openLoginCommand;
        public ICommand OpenCreateCommand => _openCreateCommand;
        public ICommand LogoutCommand => _logoutCommand;
        public ICommand OpenCloneCommand => _openCloneCommand;



        public ConnectionData ConnectionData
        {
            get => _connectionData;
            set => this.RaiseAndSetIfChanged(ref _connectionData, value);
        }

        [ImportingConstructor]
        public ConnectSectionViewModel(
            ExportFactory<ILoginDialogView> loginViewFactory,
            ExportFactory<ICloneRepositoriesDialogView> cloneRepoViewFactory,
            ExportFactory<ICreateRepositoriesDialogView> createRepoViewFactory,
            IEventAggregatorService eventAggregator,
            IUserInformationService userInformationService,
            IGitClientService gitClientService)
        {
            _loginViewFactory = loginViewFactory;
            _cloneRepoViewFactory = cloneRepoViewFactory;
            _createRepoViewFactory = createRepoViewFactory;
            _eventAggregator = eventAggregator;
            _userInformationService = userInformationService;
            _gitClientService = gitClientService;

            ConnectionData = _userInformationService.ConnectionData;
        }

        public void InitializeCommands()
        {
            _openLoginCommand = ReactiveCommand.Create(() => _loginViewFactory.CreateExport().Value.ShowDialog());
            _openCloneCommand = ReactiveCommand.Create(() => _cloneRepoViewFactory.CreateExport().Value.ShowDialog());
            _openCreateCommand = ReactiveCommand.Create(() => _createRepoViewFactory.CreateExport().Value.ShowDialog());
            _logoutCommand = ReactiveCommand.Create(() => { _gitClientService.Logout(); });
        }

        protected override IEnumerable<IDisposable> SetupObservables()
        {
            yield return _eventAggregator.GetEvent<ConnectionChangedEvent>().Subscribe(ConnectionChanged);
        }

        private void ConnectionChanged(ConnectionChangedEvent connectionChangedEvent)
        {
            ConnectionData = connectionChangedEvent.Data;
        }
    }
}
