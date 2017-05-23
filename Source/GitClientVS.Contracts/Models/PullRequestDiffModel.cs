using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Contracts.Models.Tree;
using ParseDiff;
using ReactiveUI;

namespace GitClientVS.Contracts.Models
{
    public class PullRequestDiffModel : ReactiveObject
    {
        private readonly ICommandsService _commandsService;
        private List<ITreeFile> _filesTree;
        private List<GitCommit> _commits;
        private List<GitComment> _comments;
        private List<FileDiff> _fileDiffs;
        private List<ICommentTree> _commentTree;

        public ReactiveCommand ShowDiffCommand { get; }

        public List<ITreeFile> FilesTree
        {
            get => _filesTree;
            set => this.RaiseAndSetIfChanged(ref _filesTree, value);
        }

        public List<FileDiff> FileDiffs
        {
            get => _fileDiffs;
            set => this.RaiseAndSetIfChanged(ref _fileDiffs, value);
        }

        public List<GitCommit> Commits
        {
            get => _commits;
            set => this.RaiseAndSetIfChanged(ref _commits, value);
        }

        public List<ICommentTree> CommentTree
        {
            get => _commentTree;
            set => this.RaiseAndSetIfChanged(ref _commentTree, value);
        }

        public List<GitComment> Comments
        {
            get => _comments;
            set => this.RaiseAndSetIfChanged(ref _comments, value);
        }

        public string FromCommit { get; set; }
        public string ToCommit { get; set; }


        public PullRequestDiffModel(
            ICommandsService commandsService
            )
        {
            _commandsService = commandsService;

            ShowDiffCommand = ReactiveCommand.CreateFromTask<TreeFile>(ShowDiff);
        }

        private Task ShowDiff(TreeFile file)
        {
            var fileDiffModel = new FileDiffModel()
            {
                FromCommit = FromCommit,
                ToCommit = ToCommit,
                TreeFile = file,
            };

            _commandsService.ShowDiffWindow(fileDiffModel, file.FileDiff.Id);
            return Task.CompletedTask;
        }
    }
}
