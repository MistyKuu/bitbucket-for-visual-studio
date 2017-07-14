using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using BitBucket.REST.API.Helpers;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Contracts.Models.Tree;
using ParseDiff;
using ReactiveUI;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(IPullRequestDiffViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PullRequestDiffViewModel : ViewModelBase, IPullRequestDiffViewModel
    {
        private readonly ICommandsService _commandsService;
        private readonly ITreeStructureGenerator _treeStructureGenerator;
        private List<ITreeFile> _filesTree;
        private List<GitCommit> _commits;
        private List<FileDiff> _fileDiffs;

        public ReactiveCommand ShowDiffCommand { get; set; }
        public ICommentViewModel CommentViewModel { get; }

        public List<ITreeFile> FilesTree
        {
            get => _filesTree;
            private set => this.RaiseAndSetIfChanged(ref _filesTree, value);
        }

        public List<FileDiff> FileDiffs
        {
            get => _fileDiffs;
            private set => this.RaiseAndSetIfChanged(ref _fileDiffs, value);
        }

        public List<GitCommit> Commits
        {
            get => _commits;
            private set => this.RaiseAndSetIfChanged(ref _commits, value);
        }

        public string FromCommit { get; set; }
        public string ToCommit { get; set; }


        [ImportingConstructor]
        public PullRequestDiffViewModel(
            ICommandsService commandsService,
            ICommentViewModel commentViewModel,
            ITreeStructureGenerator treeStructureGenerator
            )
        {
            _commandsService = commandsService;
            CommentViewModel = commentViewModel;
            _treeStructureGenerator = treeStructureGenerator;
        }

        public void InitializeCommands()
        {
            ShowDiffCommand = ReactiveCommand.CreateFromTask<TreeFile>(ShowDiff);
        }

        public string ErrorMessage { get; set; }
        public IEnumerable<ReactiveCommand> ThrowableCommands => new[] { ShowDiffCommand }.Concat(CommentViewModel.ThrowableCommands);


        public void AddFileDiffs(IEnumerable<FileDiff> fileDiffs)
        {
            FileDiffs = fileDiffs.ToList();
            FilesTree = _treeStructureGenerator.CreateFileTree(FileDiffs).ToList();
        }

        public async Task UpdateComments(long pullRequestId)
        {
            await CommentViewModel.UpdateComments(pullRequestId);
        }

        public void AddCommits(IEnumerable<GitCommit> commits)
        {
            Commits = commits.ToList();
        }

        private Task ShowDiff(TreeFile file)
        {
            var fileDiffModel = new FileDiffModel()
            {
                FromCommit = FromCommit,
                ToCommit = ToCommit,
                TreeFile = file,
                CommentViewModel = CommentViewModel
            };

            _commandsService.ShowDiffWindow(fileDiffModel, file.FileDiff.Id);
            return Task.CompletedTask;
        }
    }
}
