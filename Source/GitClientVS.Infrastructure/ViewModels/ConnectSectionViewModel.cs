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
using System.Linq;
using System.Reactive.Linq;
using System.Reactive;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(IConnectSectionViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConnectSectionViewModel : ViewModelBase, IConnectSectionViewModel
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
        private ReactiveCommand _initializeCommand;

        private ConnectionData _connectionData;
        private IGitService _gitService;
        private List<LocalRepo> _localRepositories;
        private LocalRepo _selectRepository;

        public ICommand OpenLoginCommand => _openLoginCommand;
        public ICommand OpenCreateCommand => _openCreateCommand;
        public ICommand LogoutCommand => _logoutCommand;
        public ICommand OpenCloneCommand => _openCloneCommand;
        public ICommand InitializeCommand => _initializeCommand;


        public LocalRepo SelectedRepository
        {
            get => _selectRepository;
            set => this.RaiseAndSetIfChanged(ref _selectRepository, value);
        }

        public ConnectionData ConnectionData
        {
            get => _connectionData;
            set => this.RaiseAndSetIfChanged(ref _connectionData, value);
        }

        public List<LocalRepo> LocalRepositories
        {
            get => _localRepositories;
            set => this.RaiseAndSetIfChanged(ref _localRepositories, value);
        }

        public string ErrorMessage { get; set; }

        public IEnumerable<ReactiveCommand> ThrowableCommands => new[] { _initializeCommand };

        [ImportingConstructor]
        public ConnectSectionViewModel(
            ExportFactory<ILoginDialogView> loginViewFactory,
            ExportFactory<ICloneRepositoriesDialogView> cloneRepoViewFactory,
            ExportFactory<ICreateRepositoriesDialogView> createRepoViewFactory,
            IEventAggregatorService eventAggregator,
            IUserInformationService userInformationService,
            IGitClientService gitClientService,
            IGitService gitService
            )
        {
            _loginViewFactory = loginViewFactory;
            _cloneRepoViewFactory = cloneRepoViewFactory;
            _createRepoViewFactory = createRepoViewFactory;
            _eventAggregator = eventAggregator;
            _userInformationService = userInformationService;
            _gitClientService = gitClientService;
            _gitService = gitService;

            ConnectionData = _userInformationService.ConnectionData;
        }

        public void InitializeCommands()
        {
            _openLoginCommand = ReactiveCommand.Create(() => _loginViewFactory.CreateExport().Value.ShowDialog());
            _openCloneCommand = ReactiveCommand.Create(() => _cloneRepoViewFactory.CreateExport().Value.ShowDialog());
            _openCreateCommand = ReactiveCommand.Create(() => _createRepoViewFactory.CreateExport().Value.ShowDialog());
            _logoutCommand = ReactiveCommand.Create(() => { _gitClientService.Logout(); });
            _initializeCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!ConnectionData.IsLoggedIn)
                    return;

                var localRepositories = _gitService.GetLocalRepositories().ToList();
                var remoteCloneUrls = (await _gitClientService.GetAllRepositories()).Select(x => x.CloneUrl).ToList();

                LocalRepositories =
                localRepositories
                .Where(x => remoteCloneUrls.Contains(x.ClonePath))
                .OrderBy(x => x.Name)
                .ToList();

            });
        }

        protected override IEnumerable<IDisposable> SetupObservables()
        {
            yield return _eventAggregator.GetEvent<ConnectionChangedEvent>().Subscribe(ConnectionChanged);

            yield return _eventAggregator.GetEvent<ActiveRepositoryChangedEvent>()
                .Select(x => Unit.Default)
                .InvokeCommand(_initializeCommand);

            yield return _eventAggregator.GetEvent<ConnectionChangedEvent>()
                .Select(x => Unit.Default)
                .InvokeCommand(_initializeCommand);
        }

        private void ConnectionChanged(ConnectionChangedEvent connectionChangedEvent)
        {
            ConnectionData = connectionChangedEvent.Data;
        }


        public void ChangeActiveRepo()
        {
            if (SelectedRepository != null)
            {
                //var opened = vsServices.TryOpenRepository(SelectedRepository.LocalPath);
                //if (!opened)
                //{
                //    // TryOpenRepository might fail because dir no longer exists. Let user find solution themselves.
                //    opened = ErrorHandler.Succeeded(ServiceProvider.GetSolution().OpenSolutionViaDlg(SelectedRepository.LocalPath, 1));
                //    if (!opened)
                //    {
                //        return false;
                //    }
                //}
            }
        }
    }
}
