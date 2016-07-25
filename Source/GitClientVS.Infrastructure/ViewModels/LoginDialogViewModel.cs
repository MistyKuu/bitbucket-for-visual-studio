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
using BitBucket.REST.API;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models;
using GitClientVS.Infrastructure.Events;
using GitClientVS.Infrastructure.Extensions;
using log4net;
using log4net.Config;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(ILoginDialogViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoginDialogViewModel : ViewModelBase, ILoginDialogViewModel
    {
        private readonly IGitClientService _bucketService;
        private readonly IEventAggregatorService _eventAggregator;
        private readonly IGitClientService _gitClientService;
        private string _login;
        private string _password;
        private readonly ReactiveCommand<Unit> _connectCommand;
        private string _errorMessage;
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public ICommand ConnectCommand => _connectCommand;

        [ImportingConstructor]
        public LoginDialogViewModel(IEventAggregatorService eventAggregator, IGitClientService gitClientService)
        {
            _eventAggregator = eventAggregator;
            _gitClientService = gitClientService;
            _connectCommand = ReactiveCommand.CreateAsyncTask(CanExecuteObservable(), _ => Connect());
            _connectCommand.Subscribe(_ => OnClose());
            this.CatchCommandErrors();
        }

        public IEnumerable<IReactiveCommand> CatchableCommands => new [] { _connectCommand };


        private async Task Connect()
        {
            await _gitClientService.LoginAsync(Login, Password);
        }

        private IObservable<bool> CanExecuteObservable()
        {
            return ValidationObservable.Select(x => CanExecute()).StartWith(CanExecute());
        }

        private bool CanExecute()
        {
            return IsObjectValid();
        }


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

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { this.RaiseAndSetIfChanged(ref _errorMessage, value); }
        }




        protected void OnClose()
        {
            Closed?.Invoke(this, new EventArgs());
        }

        public event EventHandler Closed;
    }
}
