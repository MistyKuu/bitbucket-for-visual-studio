using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GitClientVS.Contracts.Interfaces.ViewModels;
using ParseDiff;
using ReactiveUI;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(IDiffWindowControlViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DiffWindowControlViewModel : ViewModelBase, IDiffWindowControlViewModel
    {
        private ReactiveCommand<Unit> _initializeCommand;
        private string _errorMessage;
        private bool _isLoading;

        public ICommand InitializeCommand => _initializeCommand;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { this.RaiseAndSetIfChanged(ref _errorMessage, value); }
        }


        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }

        public IEnumerable<IReactiveCommand> ThrowableCommands => new[] { _initializeCommand };
        public IEnumerable<IReactiveCommand> LoadingCommands => new[] { _initializeCommand };

        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateAsyncTask(Observable.Return(true), x => ShowFileDiff((FileDiff)x));
        }

        private async Task ShowFileDiff(FileDiff fileDiff)
        {

        }
    }
}
