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
    [Export(typeof(IProxyLoginDialogViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ProxyLoginDialogViewModel : ViewModelBase, IProxyLoginDialogViewModel
    {
        private string _login;
        private string _password;
        private ReactiveCommand _acceptCommand;
        private string _errorMessage;
        private string _proxyUrl;

        public ICommand AcceptCommand => _acceptCommand;

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

        public string ProxyUrl
        {
            get => _proxyUrl;
            set
            {
                this.RaiseAndSetIfChanged(ref _proxyUrl, value);
                this.RaisePropertyChanged(nameof(Text));
            }
        }

        public string Text => $"Bitbucket extension detected proxy requiring authentication ({ProxyUrl}). Please provide proxy credentials.";

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public IEnumerable<ReactiveCommand> ThrowableCommands => new[] { _acceptCommand };

        protected override IEnumerable<IDisposable> SetupObservables()
        {
            yield break;
        }


        public void InitializeCommands()
        {
            _acceptCommand = ReactiveCommand.Create(OnClose);
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

        protected void OnClose()
        {
            Closed?.Invoke(this, true);
        }

        public event EventHandler<bool> Closed;
    }
}
