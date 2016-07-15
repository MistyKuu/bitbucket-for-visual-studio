using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using BitBucketVs.Contracts.Interfaces;
using ReactiveUI;
using System.Reactive.Linq;
using System.Security;
using System.Windows.Input;
using BitBucketVs.Contracts.Interfaces.ViewModels;
using BitBucketVs.Contracts.Interfaces.Views;

namespace BitbucketVS.Infrastructure.ViewModels
{
    [Export(typeof(ILoginDialogViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoginDialogViewModel : ViewModelBase, ILoginDialogViewModel
    {
        private readonly IBitbucketService _bucketService;
        private string _login;
        private string _password;
        private readonly ReactiveCommand<Unit> _connectCommand;
        private string _error;

        [ImportingConstructor]
        public LoginDialogViewModel(IBitbucketService bucketService)
        {
            _bucketService = bucketService;
            _connectCommand = ReactiveCommand.CreateAsyncTask(CanExecute(), async _ => await _bucketService.ConnectAsync(Login, Password));
            _connectCommand.ThrownExceptions.Subscribe(ex =>
            {
                Error = ex.Message;
            });
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
    }
}
