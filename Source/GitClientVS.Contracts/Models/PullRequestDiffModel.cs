using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Models.GitClientModels;
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
        private readonly ReactiveCommand _showDiffCommand;
        private List<ICommentTree> _commentTree;

        public PullRequestDiffModel(ICommandsService commandsService)
        {
            _commandsService = commandsService;
            _showDiffCommand = ReactiveCommand.CreateFromTask<FileDiff>(ShowDiff);
        }

        public ICommand ShowDiffCommand => _showDiffCommand;

        public List<ITreeFile> FilesTree
        {
            get { return _filesTree; }
            set { this.RaiseAndSetIfChanged(ref _filesTree, value); }
        }

        public List<FileDiff> FileDiffs
        {
            get { return _fileDiffs; }
            set { this.RaiseAndSetIfChanged(ref _fileDiffs, value); }
        }

        public List<GitCommit> Commits
        {
            get { return _commits; }
            set { this.RaiseAndSetIfChanged(ref _commits, value); }
        }

        public List<ICommentTree> CommentTree
        {
            get { return _commentTree; }
            set { this.RaiseAndSetIfChanged(ref _commentTree, value); }
        }

        public List<GitComment> Comments
        {
            get { return _comments; }
            set { this.RaiseAndSetIfChanged(ref _comments, value); }
        }

        private Task ShowDiff(FileDiff diff)
        {
            _commandsService.ShowDiffWindow(diff, diff.Id);
            return Task.CompletedTask;
        }
    }
}
