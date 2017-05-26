using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.Tree;
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
        private readonly IGitClientService _gitClientService;
        private readonly ICommandsService _commandsService;

        private ReactiveCommand _showSideBySideDiffCommand;
        private ReactiveCommand _initializeCommand;
        private ReactiveCommand _viewFileCommand;

        private string _errorMessage;
        private bool _isLoading;
        private FileDiff _fileDiff;
        private Theme _currentTheme;
        private FileDiffModel _fileDiffModel;
        public object VsFrame { get; private set; }


        public ICommand ShowSideBySideDiffCommand => _showSideBySideDiffCommand;
        public ICommand ViewFileCommand => _viewFileCommand;

        public ICommand InitializeCommand => _initializeCommand;

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        public Theme CurrentTheme
        {
            get => _currentTheme;
            set => this.RaiseAndSetIfChanged(ref _currentTheme, value);
        }


        public FileDiff FileDiff
        {
            get => _fileDiff;
            set => this.RaiseAndSetIfChanged(ref _fileDiff, value);
        }

        public IEnumerable<ReactiveCommand> ThrowableCommands => new[] { _initializeCommand, _showSideBySideDiffCommand, _viewFileCommand };
        public IEnumerable<ReactiveCommand> LoadingCommands => new[] { _initializeCommand, _showSideBySideDiffCommand, _viewFileCommand };

        [ImportingConstructor]
        public DiffWindowControlViewModel(
            IEventAggregatorService eventAggregator,
            IUserInformationService userInfoService,
            IGitClientService gitClientService,
            ICommandsService commandsService)
        {
            _eventAggregator = eventAggregator;
            _userInfoService = userInfoService;
            _gitClientService = gitClientService;
            _commandsService = commandsService;
            CurrentTheme = _userInfoService.CurrentTheme;
        }

        protected override IEnumerable<IDisposable> SetupObservables()
        {
            yield return _eventAggregator.GetEvent<ThemeChangedEvent>().Subscribe(ev => CurrentTheme = ev.Theme);
        }

        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateFromTask<FileDiffModel>(ShowFileDiff, Observable.Return(true));
            _showSideBySideDiffCommand = ReactiveCommand.CreateFromTask(ShowSideBySideDiff);
            _viewFileCommand = ReactiveCommand.CreateFromTask(ViewFile);
        }

        private Task ShowFileDiff(FileDiffModel fileDiffModel)
        {
            _fileDiffModel = fileDiffModel;
            FileDiff = fileDiffModel.TreeFile.FileDiff;
            return Task.CompletedTask;
        }

        private Task ViewFile()
        {
            throw new NotImplementedException();
        }

        private async Task ShowSideBySideDiff()
        {
            var results = await Task.WhenAll(
                GetFileContent(_fileDiffModel.ToCommit, FileDiff.DisplayFileName),
                GetFileContent(_fileDiffModel.FromCommit, FileDiff.DisplayFileName)
            );

            VsFrame = _commandsService.ShowSideBySideDiffWindow(
                results[0],
                results[1],
                $"{FileDiff.DisplayFileName} ({_fileDiffModel.ToCommit})",
                $"{FileDiff.DisplayFileName} ({_fileDiffModel.FromCommit})",
                $"Side-by-side Diff ({FileDiff.DisplayFileName})",
                $"Side-by-side Diff ({FileDiff.DisplayFileName})",
                VsFrame
            );
        }

        private async Task<string> GetFileContent(string commit, string fileName)
        {
            try
            {
                return await _gitClientService.GetFileContent(commit, fileName);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
