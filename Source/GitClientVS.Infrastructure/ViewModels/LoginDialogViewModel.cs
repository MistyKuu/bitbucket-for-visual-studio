using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
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
using GitClientVS.Contracts.Models;
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
        private readonly IEventAggregatorService _eventAggregator;
        private string _login;
        private string _password;
        private readonly ReactiveCommand<Unit> _connectCommand;
        private string _loginError;
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        [ImportingConstructor]
        public LoginDialogViewModel(
            IBitbucketService bucketService,
            IEventAggregatorService eventAggregator)
        {
            _bucketService = bucketService;
            _eventAggregator = eventAggregator;
            _connectCommand = ReactiveCommand.CreateAsyncTask(CanExecuteObservable(), _ => Connect());

            _connectCommand.ThrownExceptions.Subscribe(OnError);

        }

        private void OnError(Exception ex)
        {
            LoginError = ex.Message;
        }

        private async Task Connect()
        {
            await _bucketService.ConnectAsync(Login, Password);
            _eventAggregator.Publish(new ConnectionChangedEvent(ConnectionData.Create(Login, Password)));
            OnClose();
        }

        private IObservable<bool> CanExecuteObservable()
        {
            return this.ValidationObservable.Select(x => CanExecute()).StartWith(CanExecute());
        }

        private bool CanExecute()
        {
            return IsObjectValid();
        }

        public ICommand ConnectCommand => _connectCommand;

        [Required]
        public string Login
        {
            get { return _login; }
            set { this.RaiseAndSetIfChanged(ref _login, value); }
        }

        [Required]
        public string Password
        {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }

        public string LoginError
        {
            get { return _loginError; }
            set { this.RaiseAndSetIfChanged(ref _loginError, value); }
        }


        protected void OnClose()
        {
            Closed?.Invoke(this, new EventArgs());
        }

        public event EventHandler Closed;
    }
}
