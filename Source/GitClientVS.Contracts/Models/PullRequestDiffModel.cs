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
        private readonly IVsTools _vsTools;
        private readonly IGitClientService _gitClientService;
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
            ICommandsService commandsService,
            IVsTools vsTools,
            IGitClientService gitClientService
            )
        {
            _commandsService = commandsService;
            _vsTools = vsTools;
            _gitClientService = gitClientService;

            ShowDiffCommand = ReactiveCommand.CreateFromTask<TreeFile>(ShowDiff);
        }

        private async Task ShowDiff(TreeFile file)
        {
            var content1 = await GetFileContent(ToCommit, file.FileDiff.DisplayFileName);
            var content2 = await GetFileContent(FromCommit, file.FileDiff.DisplayFileName);

            _vsTools.RunDiff(content1, content2, $"{file.FileDiff.DisplayFileName} ({ToCommit})", $"{file.FileDiff.DisplayFileName} ({FromCommit})");

            // _commandsService.ShowDiffWindow(file.FileDiff, file.FileDiff.Id);
        }

        private async Task<string> GetFileContent(string commit, string fileName)
        {
            try
            {
                return await _gitClientService.GetFileContent(commit, fileName);
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
    }
}
