using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive.Linq;
using System.Windows.Input;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models;
using log4net;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(ILoginDialogViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoginDialogViewModel : ViewModelBase, ILoginDialogViewModel
    {
        private readonly IGitClientService _gitClientService;
        private string _login;
        private string _password;
        private ReactiveCommand _connectCommand;
        private string _errorMessage;
        private bool _isLoading;
        private string _host;
        private bool _isEnterprise;

        public ICommand ConnectCommand => _connectCommand;
        public IEnumerable<ReactiveCommand> ThrowableCommands => new List<ReactiveCommand> { _connectCommand };
        public IEnumerable<ReactiveCommand> LoadingCommands => new[] { _connectCommand };

        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }
        
        [Required]
        public string Login
        {
            get => _login;
            set => this.RaiseAndSetIfChanged(ref _login, value);
        }

        [Required]
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        [Required]
        public bool IsEnterprise
        {
            get => _isEnterprise;
            set => this.RaiseAndSetIfChanged(ref _isEnterprise, value);
        }

        [ValidatesViaMethod(AllowBlanks = true, AllowNull = true, Name = nameof(ValidateHost), ErrorMessage = "Url is not valid. It must include schema.")]
        public string Host
        {
            get => _host;
            set => this.RaiseAndSetIfChanged(ref _host, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }


        [ImportingConstructor]
        public LoginDialogViewModel(IGitClientService gitClientService)
        {
            _gitClientService = gitClientService;
            IsEnterprise = false;
        }

        protected override IEnumerable<IDisposable> SetupObservables()
        {
            this.WhenAnyValue(x => x.IsEnterprise).Subscribe(_ => ForcePropertyValidation(nameof(Host)));
            yield break;
        }


        public void InitializeCommands()
        {
            _connectCommand = ReactiveCommand.CreateFromTask(_ => Connect(), CanExecuteObservable());
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
            OnClose();
        }


        private IObservable<bool> CanExecuteObservable()
        {
            return ValidationObservable.Select(x => Unit.Default)
                .Merge(Changed.Select(x => Unit.Default))
                .Select(x => CanExecute()).StartWith(CanExecute());
        }

        private bool CanExecute()
        {
            return IsObjectValid();
        }

        public bool ValidateHost(string host)
        {
            if (!IsEnterprise)
                return true;
            

            return (Uri.TryCreate(host, UriKind.Absolute, out Uri outUri) &&
                    (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps));
        }

        protected void OnClose()
        {
            Closed?.Invoke(this, new EventArgs());
        }

        public event EventHandler Closed;
    }
}
