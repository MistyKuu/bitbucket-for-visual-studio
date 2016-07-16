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
        private readonly ReactiveCommand<object> _openLoginCommand;
        private readonly ReactiveCommand<object> _logoutCommand;
        private bool _isLoggedIn;
        private IDisposable _observable;

        public ICommand OpenLoginCommand => _openLoginCommand;
        public ICommand LogoutCommand => _logoutCommand;

        [ImportingConstructor]
        public ConnectSectionViewModel(ExportFactory<ILoginDialogView> loginViewFactory, IEventAggregatorService eventAggregator)
        {
            _loginViewFactory = loginViewFactory;
            _eventAggregator = eventAggregator;

            _openLoginCommand = ReactiveCommand.Create(CanExecuteOpenLogin());
            _logoutCommand = ReactiveCommand.Create();

            SetupObservables();
        }

        private void SetupObservables()
        {
            _openLoginCommand.Subscribe(_ => _loginViewFactory.CreateExport().Value.ShowModal());
            _logoutCommand.Subscribe(_ => _eventAggregator.Publish(new ConnectionChangedEvent(ConnectionData.NotLogged)));

            _observable = _eventAggregator.GetEvent<ConnectionChangedEvent>().Subscribe(ConnectionChanged);
        }

        private void ConnectionChanged(ConnectionChangedEvent connectionChangedEvent)
        {
            IsLoggedIn = connectionChangedEvent.Data.IsLoggedIn;
        }

        private IObservable<bool> CanExecuteOpenLogin()
        {
            return Observable.Return(true);
        }

        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set { this.RaiseAndSetIfChanged(ref _isLoggedIn, value); }
        }

        public void Dispose()
        {
            _observable.Dispose();
        }
    }
}
