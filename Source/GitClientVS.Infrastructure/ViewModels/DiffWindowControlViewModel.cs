using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BitBucket.REST.API.Helpers;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
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
        private ObservableCollection<object> _displayedModels;
        private ICommentViewModel _commentViewModel;


        public ICommand ShowSideBySideDiffCommand => _showSideBySideDiffCommand;
        public ICommand ViewFileCommand => _viewFileCommand;
        public ICommand InitializeCommand => _initializeCommand;


        public ObservableCollection<object> DisplayedModels
        {
            get => _displayedModels;
            set => this.RaiseAndSetIfChanged(ref _displayedModels, value);
        }

        public object VsFrame { get; private set; }

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

        public ICommentViewModel CommentViewModel
        {
            get => _commentViewModel;
            set => this.RaiseAndSetIfChanged(ref _commentViewModel, value);
        }

        public IEnumerable<ReactiveCommand> ThrowableCommands => new[] { _initializeCommand, _showSideBySideDiffCommand, _viewFileCommand };
        public IEnumerable<ReactiveCommand> LoadingCommands => new[] { _initializeCommand, _showSideBySideDiffCommand, _viewFileCommand };

        [ImportingConstructor]
        public DiffWindowControlViewModel(
            IEventAggregatorService eventAggregator,
            IUserInformationService userInfoService,
            IGitClientService gitClientService,
            ICommandsService commandsService
            )
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
            this.WhenAnyValue(x => x.CommentViewModel, x => x.CommentViewModel.InlineCommentTree).Subscribe(_ => PrepareDiffModels());
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
            CommentViewModel = fileDiffModel.CommentViewModel;

            return Task.CompletedTask;
        }

        private void PrepareDiffModels()
        {
            DisplayedModels = new ObservableCollection<object>();

            var topLevelFileComments = _fileDiffModel
                                           .CommentViewModel
                                           .InlineCommentTree?
                                           .Where(x => !x.AllDeleted)
                                           .Where(x => x.Comment.Inline.Path == FileDiff.From || x.Comment.Inline.Path == FileDiff.To)
                                           .ToList() ?? new List<ICommentTree>();

            var fileLevelComment = topLevelFileComments
                .Where(x=>!x.Comment.IsDeleted)
                .FirstOrDefault(x => x.Comment.Inline.From == null && x.Comment.Inline.To == null);

            if (fileLevelComment != null)
                DisplayedModels.Add(fileLevelComment);

            foreach (var chunk in FileDiff.Chunks)
            {
                var splitChanges = chunk.Changes.Where(
                    x => topLevelFileComments.Any(ch => ch.Comment.Inline.From == x.OldIndex && ch.Comment.Inline.To == x.NewIndex)).ToList();

                SplitChunkByIndexes(chunk, splitChanges, topLevelFileComments);
            }
        }

        private void SplitChunkByIndexes(ChunkDiff chunk, List<LineDiff> splitChanges, List<ICommentTree> topLevelFileComments)
        {
            foreach (var change in splitChanges)
            {
                var currentChangeIndex = chunk.Changes.IndexOf(change);
                var currentComments = topLevelFileComments.Where(x => x.Comment.Inline.From == change.OldIndex && x.Comment.Inline.To == change.NewIndex).ToList();

                var firstChunk = new ChunkDiff { Changes = chunk.Changes.Take(currentChangeIndex + 1).ToList() };
                var secondChunk = new ChunkDiff { Changes = chunk.Changes.Skip(currentChangeIndex + 1).ToList() };

                DisplayedModels.Add(firstChunk);

                foreach (var comment in currentComments)
                    DisplayedModels.Add(comment);

                chunk = secondChunk;
            }

            if (chunk.Changes.Any())
                DisplayedModels.Add(chunk);
        }

        private Task ViewFile()
        {
            throw new NotImplementedException();
        }

        private async Task ShowSideBySideDiff()
        {
            Task<string> t1;
            Task<string> t2;

            if (FileDiff.Type == FileChangeType.Modified)
            {
                t1 = GetFileContent(_fileDiffModel.ToCommit, FileDiff.DisplayFileName);
                t2 = GetFileContent(_fileDiffModel.FromCommit, FileDiff.DisplayFileName);
            }
            else if (FileDiff.Type == FileChangeType.Add)
            {
                t1 = Task.FromResult(string.Empty);
                t2 = GetFileContent(_fileDiffModel.FromCommit, FileDiff.DisplayFileName);
            }
            else
            {
                t1 = GetFileContent(_fileDiffModel.ToCommit, FileDiff.DisplayFileName);
                t2 = Task.FromResult(string.Empty);
            }

            var results = await Task.WhenAll(t1, t2);

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
