using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces;
using ReactiveUI;
using System.Reactive.Linq;
using System.Security;
using System.Windows.Input;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Infrastructure.Events;
using log4net;
using log4net.Config;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(ILoginDialogViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoginDialogViewModel : ViewModelBase, ILoginDialogViewModel
    {
        private readonly IBitbucketService _bucketService;
        private readonly IUserInformationService _userInformationService;
        private readonly IEventAggregatorService _eventAggregator;
        private string _login;
        private string _password;
        private readonly ReactiveCommand<Unit> _connectCommand;
        private string _error;
        private readonly log4net.ILog _logger;


        [ImportingConstructor]
        public LoginDialogViewModel(IBitbucketService bucketService, IUserInformationService userInformationService, IEventAggregatorService eventAggregator)
        {
            _bucketService = bucketService;
            _userInformationService = userInformationService;
            _eventAggregator = eventAggregator;
            _connectCommand = ReactiveCommand.CreateAsyncTask(CanExecute(), _ => Connect());
            _connectCommand.Subscribe(_ => OnLoggedIn());

            _connectCommand.ThrownExceptions.Subscribe(OnError);
            _logger =  LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            _logger.Info("Creating ");
        }

        private void OnError(Exception ex)
        {
            Error = ex.Message;
        }

        private void OnLoggedIn()
        {
            _eventAggregator.Publish(new ConnectionChangedEvent() { IsLoggedIn = true });
            OnClose();
        }

        private async Task Connect()
        {
            await _bucketService.ConnectAsync(Login, Password);
        }

        private IObservable<bool> CanExecute()
        {
            return Observable.Return(true);
        }

        public ICommand ConnectCommand => _connectCommand;

        public string Login
        {
            get { return _login; }
            set { this.RaiseAndSetIfChanged(ref _login, value); }
        }

        public string Password
        {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }

        public string Error
        {
            get { return _error; }
            set { this.RaiseAndSetIfChanged(ref _error, value); }
        }


        protected void OnClose()
        {
            Closed?.Invoke(this, new EventArgs());
        }

        public event EventHandler Closed;
    }
}
