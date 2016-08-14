using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using ParseDiff;
using ReactiveUI;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(IPullRequestsDetailViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PullRequestsDetailViewModel : ViewModelBase, IPullRequestsDetailViewModel
    {
        private readonly IGitClientService _gitClientService;
        private readonly IGitService _gitService;
        private readonly ICommandsService _commandsService;
        private readonly IDiffFileParser _diffFileParser;
        private string _errorMessage;
        private bool _isLoading;
        private ReactiveCommand<Unit> _initializeCommand;
        private IEnumerable<GitCommit> _commits;
        private IEnumerable<GitComment> _comments;
        private ReactiveCommand<Unit> _showDiffCommand;
        private IEnumerable<FileDiff> _fileDiffs;
        private List<ITreeFile> _filesTree;

        [ImportingConstructor]
        public PullRequestsDetailViewModel(
            IGitClientService gitClientService,
            IGitService gitService,
            ICommandsService commandsService,
            IDiffFileParser diffFileParser
            )
        {
            _gitClientService = gitClientService;
            _gitService = gitService;
            _commandsService = commandsService;
            _diffFileParser = diffFileParser;
        }

        public ICommand InitializeCommand => _initializeCommand;
        public ICommand ShowDiffCommand => _showDiffCommand;

        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateAsyncTask(Observable.Return(true), x => LoadPullRequestData((GitPullRequest)x));
            _showDiffCommand = ReactiveCommand.CreateAsyncTask(Observable.Return(true), (x) => ShowDiff((FileDiff)x));
        }

        private async Task ShowDiff(FileDiff diff)
        {
            _commandsService.ShowDiffWindow(diff);
        }

        public IEnumerable<GitComment> Comments
        {
            get { return _comments; }
            set { this.RaiseAndSetIfChanged(ref _comments, value); }
        }

        public IEnumerable<GitCommit> Commits
        {
            get { return _commits; }
            set { this.RaiseAndSetIfChanged(ref _commits, value); }
        }

        public IEnumerable<FileDiff> FileDiffs
        {
            get { return _fileDiffs; }
            set { this.RaiseAndSetIfChanged(ref _fileDiffs, value); }
        }

        public List<ITreeFile> FilesTree
        {
            get { return _filesTree; }
            set { this.RaiseAndSetIfChanged(ref _filesTree, value); }
        }

        private async Task LoadPullRequestData(GitPullRequest pr)
        {
            var id = pr.Id;
            var currentRepository = _gitService.GetActiveRepository();
            Commits = await _gitClientService.GetPullRequestCommits("django-piston", "jespern", id);
            Comments = await _gitClientService.GetPullRequestComments("django-piston", "jespern", id);
            var diff = await _gitClientService.GetPullRequestDiff("django-piston", "jespern", id);
            FileDiffs = _diffFileParser.Parse(diff).ToList();
            CreateFileTree(FileDiffs.ToList());
        }

        public void CreateFileTree(List<FileDiff> fileDiffs, string rootFileName = "test", char separator = '/')
        {
            var entryFile = new TreeDirectory(rootFileName);

            foreach (var fileDiff in fileDiffs.Where(x => !string.IsNullOrEmpty(x.From.Trim())))
            {
                ITreeFile currentFile = entryFile;
                
                var pathChunks = fileDiff.From.Split(separator);
                var lastItem = pathChunks.Last();
                foreach (var pathChunk in pathChunks)
                {
                    var tmp = currentFile.Files.Where(x => x.Name.Equals(pathChunk));
                    if (tmp.Count() > 0)
                    {
                        currentFile = tmp.Single();
                    }
                    else
                    {
                        ITreeFile newItem;
                        if (lastItem.Equals(pathChunk))
                        {
                            newItem = new TreeFile(pathChunk, fileDiff);
                        }
                        else
                        {
                            newItem = new TreeDirectory(pathChunk);
                        }
                       
                        currentFile.Files.Add(newItem);
                        currentFile = newItem;
                    }
                }
            }

           
            FilesTree = entryFile.Files;
        }


        public IEnumerable<IReactiveCommand> ThrowableCommands => new[] { _initializeCommand, _showDiffCommand };
        public IEnumerable<IReactiveCommand> LoadingCommands => new[] { _initializeCommand, _showDiffCommand };

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                this.RaiseAndSetIfChanged(ref _errorMessage, value);
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }

    }
}
