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
using GitClientVS.Contracts;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(IConnectSectionViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ConnectSectionViewModel : ViewModelBase, IConnectSectionViewModel
    {
        private readonly ExportFactory<ILoginDialogView> _loginBitbucketViewFactory;
        private readonly ExportFactory<ICloneRepositoriesDialogView> _cloneRepoViewFactory;
        private readonly ExportFactory<ICreateRepositoriesDialogView> _createRepoViewFactory;
        private readonly IEventAggregatorService _eventAggregator;
        private readonly IUserInformationService _userInformationService;
        private readonly IGitClientService _gitClientService;
        private readonly ReactiveCommand<object> _openLoginBitbucketCommand;
        private readonly ReactiveCommand<object> _openLoginGitlabCommand;
        private readonly ReactiveCommand<object> _logoutCommand;
        private readonly ReactiveCommand<object> _openCloneCommand;
        private ReactiveCommand<object> _openCreateCommand;

        private IDisposable _observable;
        private ConnectionData _connectionData;

        public ICommand OpenLoginBitbucketCommand => _openLoginBitbucketCommand;
        public ICommand OpenLoginGitlabCommand => _openLoginGitlabCommand;
        public ICommand OpenCreateCommand => _openCreateCommand;
        public ICommand LogoutCommand => _logoutCommand;
        public ICommand OpenCloneCommand => _openCloneCommand;


        [ImportingConstructor]
        public ConnectSectionViewModel(
            ExportFactory<ILoginDialogView> loginBitbucketViewFactory,
            ExportFactory<ICloneRepositoriesDialogView> cloneRepoViewFactory,
            ExportFactory<ICreateRepositoriesDialogView> createRepoViewFactory,
            IEventAggregatorService eventAggregator,
            IUserInformationService userInformationService,
            IGitClientServiceFactory gitClientFactory)
        {
            _loginBitbucketViewFactory = loginBitbucketViewFactory;
            _cloneRepoViewFactory = cloneRepoViewFactory;
            _createRepoViewFactory = createRepoViewFactory;
            _eventAggregator = eventAggregator;
            _userInformationService = userInformationService;
            _gitClientService = gitClientFactory.GetService();

            _openLoginBitbucketCommand = ReactiveCommand.Create(Observable.Return(true));
            _openLoginGitlabCommand = ReactiveCommand.Create(Observable.Return(true));
            _openCloneCommand = ReactiveCommand.Create(Observable.Return(true));
            _openCreateCommand = ReactiveCommand.Create(Observable.Return(true));
            _logoutCommand = ReactiveCommand.Create();

            ConnectionData = _userInformationService.ConnectionData;
            AvailableServices = gitClientFactory.AvailableServices;

            SetupObservables();
        }


        private void SetupObservables()
        {
            _openLoginBitbucketCommand.Subscribe(_ => { OpenLoginWindow(GitProviderType.Bitbucket); });
            _openLoginGitlabCommand.Subscribe(_ => { OpenLoginWindow(GitProviderType.Gitlab); });
            _openCreateCommand.Subscribe(_ => _createRepoViewFactory.CreateExport().Value.ShowDialog());
            _openCloneCommand.Subscribe(_ => _cloneRepoViewFactory.CreateExport().Value.ShowDialog());
            _logoutCommand.Subscribe(_ => { _gitClientService.Logout(); });

            _observable = _eventAggregator.GetEvent<ConnectionChangedEvent>().Subscribe(ConnectionChanged);
        }

        private void OpenLoginWindow(GitProviderType gitProviderType)
        {
            var view = _loginBitbucketViewFactory.CreateExport().Value;
            view.InitializeCommand.Execute(gitProviderType);
            view.ShowDialog();
        }

        private void ConnectionChanged(ConnectionChangedEvent connectionChangedEvent)
        {
            ConnectionData = connectionChangedEvent.Data;
        }


        public IEnumerable<GitProviderType> AvailableServices { get; set; }

        public ConnectionData ConnectionData
        {
            get { return _connectionData; }
            set { this.RaiseAndSetIfChanged(ref _connectionData, value); }
        }


        public void Dispose()
        {
            _observable.Dispose();
        }
    }
}
