using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using BitBucketVs.Contracts.Interfaces;
using ReactiveUI;
using System.Reactive.Linq;
using System.Windows.Input;
using BitBucketVs.Contracts.Interfaces.ViewModels;
using BitBucketVs.Contracts.Interfaces.Views;

namespace BitbucketVS.Infrastructure.ViewModels
{
    [Export(typeof(IConnectSectionViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ConnectSectionViewModel : ViewModelBase, IConnectSectionViewModel
    {
        private readonly ILoginDialogView _loginView;
        private readonly ReactiveCommand<object> _openConnectCommand;

        [ImportingConstructor]
        public ConnectSectionViewModel(
            ILoginDialogView loginView, 
            ILoginDialogViewModel loginViewModel)
        {
            this.WhenAnyValue(x => x.Message).Subscribe(x =>
            {
                MessageB = Message + " Hej";
            });
            Message = "Bucket";

            _loginView = loginView;
            _loginView.DataContext = loginViewModel;

            _openConnectCommand = ReactiveCommand.Create(CanExecute());
            _openConnectCommand.Subscribe(_ => _loginView.ShowModal());
        }

        private IObservable<bool> CanExecute()
        {
            return Observable.Return(true);
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { this.RaiseAndSetIfChanged(ref _message, value); }
        }

        public string MessageB { get; set; }

        public ICommand OpenConnectCommand => _openConnectCommand;
    }
}
