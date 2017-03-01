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
using log4net;
using log4net.Config;
using GitClientVS.Infrastructure.Extensions;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(ILoginDialogViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoginDialogViewModel : ViewModelBase, ILoginDialogViewModel
    {
        private readonly IEventAggregatorService _eventAggregator;
        private readonly IGitClientService _gitClientService;
        private readonly IUserInformationService _userInformationService;
        private string _login;
        private string _password;
        private ReactiveCommand<Unit> _connectCommand;
        private string _errorMessage;
        private bool _isLoading;
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _host;
        private bool _isEnterprise;

        public ICommand ConnectCommand => _connectCommand;
        public IEnumerable<IReactiveCommand> ThrowableCommands => new List<IReactiveCommand> { _connectCommand };
        public IEnumerable<IReactiveCommand> LoadingCommands => new[] { _connectCommand };

        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }


        [ImportingConstructor]
        public LoginDialogViewModel(
            IEventAggregatorService eventAggregator,
            IGitClientService gitClientService,
            IUserInformationService userInformationService)
        {
            _eventAggregator = eventAggregator;
            _gitClientService = gitClientService;
            _userInformationService = userInformationService;
            IsEnterprise = false;
            SetupObservables();
        }

        private void SetupObservables()
        {
            _connectCommand.Subscribe(_ => OnClose());
            this.WhenAnyValue(x => x.IsEnterprise).Subscribe(_ => ForcePropertyValidation(nameof(Host)));
        }


        public void InitializeCommands()
        {
            _connectCommand = ReactiveCommand.CreateAsyncTask(CanExecuteObservable(), _ => Connect());
        }

        private async Task Connect()
        {
            var cred = new GitCredentials()
            {
                Login = Login,
                Password = Password,
                Host = IsEnterprise ? new Uri(Host) : null,
                IsEnterprise = IsEnterprise
            };

            await _gitClientService.LoginAsync(cred);
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

        [Required]
        public bool IsEnterprise
        {
            get { return _isEnterprise; }
            set { this.RaiseAndSetIfChanged(ref _isEnterprise, value); }
        }

        [ValidatesViaMethod(AllowBlanks = true, AllowNull = true, Name = nameof(ValidateHost), ErrorMessage = "Url is not valid. It must include schema.")]
        public string Host
        {
            get { return _host; }
            set { this.RaiseAndSetIfChanged(ref _host, value); }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { this.RaiseAndSetIfChanged(ref _errorMessage, value); }
        }

        public bool ValidateHost(string host)
        {
            if (!IsEnterprise)
                return true;

            Uri outUri;

            return (Uri.TryCreate(host, UriKind.Absolute, out outUri) &&
                    (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps));
        }

        protected void OnClose()
        {
            Closed?.Invoke(this, new EventArgs());
        }

        public event EventHandler Closed;
    }
}
