using System;
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
        private readonly IGitClientService _gitClientService;
        private readonly ITreeStructureGenerator _treeStructureGenerator;
        private List<ITreeFile> _filesTree;
        private List<GitCommit> _commits;
        private List<FileDiff> _fileDiffs;
        private List<ICommentTree> _commentTree;
        private List<ICommentTree> _inlineCommentTree;
        private int _commentsCount;

        public ReactiveCommand ShowDiffCommand { get; set; }
        public ReactiveCommand ReplyCommentCommand { get; set; }
        public ReactiveCommand EditCommentCommand { get; set; }
        public ReactiveCommand DeleteCommentCommand { get; set; }

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

        public List<ICommentTree> CommentTree
        {
            get => _commentTree;
            private set => this.RaiseAndSetIfChanged(ref _commentTree, value);
        }

        public int CommentsCount
        {
            get => _commentsCount;
            private set => this.RaiseAndSetIfChanged(ref _commentsCount, value);
        }

        public List<ICommentTree> InlineCommentTree
        {
            get => _inlineCommentTree;
            private set => this.RaiseAndSetIfChanged(ref _inlineCommentTree, value);
        }

        public long Id { get; set; }
        public string FromCommit { get; set; }
        public string ToCommit { get; set; }


        [ImportingConstructor]
        public PullRequestDiffViewModel(ICommandsService commandsService, IGitClientService gitClientService, ITreeStructureGenerator treeStructureGenerator)
        {
            _commandsService = commandsService;
            _gitClientService = gitClientService;
            _treeStructureGenerator = treeStructureGenerator;
        }

        public void InitializeCommands()
        {
            ShowDiffCommand = ReactiveCommand.CreateFromTask<TreeFile>(ShowDiff);
            ReplyCommentCommand = ReactiveCommand.CreateFromTask<ICommentTree>(ReplyToComment);
            EditCommentCommand = ReactiveCommand.CreateFromTask<ICommentTree>(EditComment);
            DeleteCommentCommand = ReactiveCommand.CreateFromTask<ICommentTree>(DeleteComment);
        }

        public string ErrorMessage { get; set; }
        public IEnumerable<ReactiveCommand> ThrowableCommands => new[] { ShowDiffCommand, ReplyCommentCommand, EditCommentCommand, DeleteCommentCommand };


        public void AddFileDiffs(IEnumerable<FileDiff> fileDiffs)
        {
            FileDiffs = fileDiffs.ToList();
            FilesTree = _treeStructureGenerator.CreateFileTree(FileDiffs).ToList();
        }

        public void AddComments(IEnumerable<GitComment> pqComments)
        {
            var inlineComments = pqComments.Where(comment => comment.IsInline).ToList();
            var notInlineComments = pqComments.Where(x => !x.IsInline).ToList();

            InlineCommentTree = _treeStructureGenerator.CreateCommentTree(inlineComments).ToList();
            CommentTree = _treeStructureGenerator.CreateCommentTree(notInlineComments).ToList();
            CommentsCount = notInlineComments.Count(x => !x.IsDeleted);
        }

        public void AddCommits(IEnumerable<GitCommit> commits)
        {
            Commits = commits.ToList();
        }

        private async Task ReplyToComment(ICommentTree commentTree)
        {
            var comment = commentTree.Comment;

            var newComment = new GitComment()
            {
                Content = comment.Content,
                Parent = new GitCommentParent() { Id = comment.Id },
                From = comment.From,
                To = comment.To,
                Path = comment.Path
            };

            await _gitClientService.AddPullRequestComment(Id, newComment);
            var comments = await _gitClientService.GetPullRequestComments(Id);
            AddComments(comments);//todo temp solution just to make it work
        }

        private Task EditComment(ICommentTree commentTreee)
        {
            throw new NotImplementedException();
        }

        private async Task DeleteComment(ICommentTree commentTree)
        {
            var comment = commentTree.Comment;
            await _gitClientService.DeletePullRequestComment(Id, comment.Id, comment.Version);
            var comments = await _gitClientService.GetPullRequestComments(Id);
            AddComments(comments); //todo temp solution just to make it work
        }

        private Task ShowDiff(TreeFile file)
        {
            var fileDiffModel = new FileDiffModel()
            {
                FromCommit = FromCommit,
                ToCommit = ToCommit,
                TreeFile = file,
                InlineCommentTree = InlineCommentTree
            };

            _commandsService.ShowDiffWindow(fileDiffModel, file.FileDiff.Id);
            return Task.CompletedTask;
        }
    }
}
