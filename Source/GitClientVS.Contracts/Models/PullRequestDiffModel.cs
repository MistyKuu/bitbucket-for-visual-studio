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
            var content1 = await GetFileContent(ToCommit, file);
            var content2 = await GetFileContent(FromCommit, file);

            Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "GITCLIENT"));

            var tempPath1 = Path.Combine(Path.GetTempPath(), "GITCLIENT", Guid.NewGuid().ToString());
            var tempPath2 = Path.Combine(Path.GetTempPath(), "GITCLIENT", Guid.NewGuid().ToString());

            File.WriteAllText(tempPath1, content1);
            File.WriteAllText(tempPath2, content2);

            _vsTools.RunDiff(tempPath1, tempPath2, $"{file.Name} ({ToCommit})", $"{file.Name} ({FromCommit})");

            // _commandsService.ShowDiffWindow(file.FileDiff, file.FileDiff.Id);
        }

        private async Task<string> GetFileContent(string commit, TreeFile file)
        {
            try
            {
                return await _gitClientService.GetFileContent(commit, file.Name);
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
    }
}
