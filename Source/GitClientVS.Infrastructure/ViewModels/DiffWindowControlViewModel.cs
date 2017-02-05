using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models;
using ParseDiff;
using ReactiveUI;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(IDiffWindowControlViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DiffWindowControlViewModel : ViewModelBase, IDiffWindowControlViewModel
    {
        private readonly IEventAggregatorService _eventAggregator;
        private readonly IUserInformationService _userInfoService;
        private ReactiveCommand<Unit> _initializeCommand;
        private string _errorMessage;
        private bool _isLoading;
        private FileDiff _fileDiff;
        private Theme _currentTheme;


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

        public Theme CurrentTheme
        {
            get { return _currentTheme; }
            set { this.RaiseAndSetIfChanged(ref _currentTheme, value); }
        }


        public FileDiff FileDiff
        {
            get { return _fileDiff; }
            set { this.RaiseAndSetIfChanged(ref _fileDiff, value); }
        }

        public IEnumerable<IReactiveCommand> ThrowableCommands => new[] { _initializeCommand };
        public IEnumerable<IReactiveCommand> LoadingCommands => new[] { _initializeCommand };

        [ImportingConstructor]
        public DiffWindowControlViewModel(IEventAggregatorService eventAggregator, IUserInformationService userInfoService)
        {
            _eventAggregator = eventAggregator;
            _userInfoService = userInfoService;
            _eventAggregator.GetEvent<ThemeChangedEvent>().Subscribe(ev => CurrentTheme = ev.Theme);
            CurrentTheme = _userInfoService.CurrentTheme;
        }


        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateAsyncTask(Observable.Return(true), x => ShowFileDiff((FileDiff)x));
        }

        private Task ShowFileDiff(FileDiff fileDiff)
        {
            FileDiff = fileDiff;
            return Task.CompletedTask;
        }
    }
}
