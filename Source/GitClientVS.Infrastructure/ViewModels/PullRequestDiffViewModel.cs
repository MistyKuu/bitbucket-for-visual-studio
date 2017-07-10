using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using BitBucket.REST.API.Helpers;
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
        private List<ITreeFile> _filesTree;
        private List<GitCommit> _commits;
        private List<FileDiff> _fileDiffs;
        private List<ICommentTree> _commentTree;
        private List<ICommentTree> _inlineCommentTree;

        public ReactiveCommand ShowDiffCommand { get; set; }
        public ReactiveCommand ReplyCommentCommand { get; set; }
        public ReactiveCommand EditCommentCommand { get; set; }
        public ReactiveCommand DeleteCommentCommand { get; set; }

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

        public long Id { get; set; }
        public string FromCommit { get; set; }
        public string ToCommit { get; set; }

        public int CommentsCount { get; set; }

        public List<ICommentTree> InlineCommentTree
        {
            get => _inlineCommentTree;
            set => this.RaiseAndSetIfChanged(ref _inlineCommentTree, value);
        }

        [ImportingConstructor]
        public PullRequestDiffViewModel(ICommandsService commandsService, IGitClientService gitClientService)
        {
            _commandsService = commandsService;
            _gitClientService = gitClientService;
            this.WhenAnyValue(x => x.CommentTree).Subscribe(_ => CommentsCount = CommentTree.Flatten(y => true, y => y.Comments)?.Count() ?? 0);
        }

        public void InitializeCommands()
        {
            ShowDiffCommand = ReactiveCommand.CreateFromTask<TreeFile>(ShowDiff);
            ReplyCommentCommand = ReactiveCommand.CreateFromTask<GitComment>(ReplyToComment);
            EditCommentCommand = ReactiveCommand.CreateFromTask<GitComment>(EditComment);
            DeleteCommentCommand = ReactiveCommand.CreateFromTask<GitComment>(DeleteComment);
        }

        public string ErrorMessage { get; set; }
        public IEnumerable<ReactiveCommand> ThrowableCommands => new[] { ShowDiffCommand, ReplyCommentCommand, EditCommentCommand, DeleteCommentCommand };

        private async Task ReplyToComment(GitComment comment)
        {
            var newComment = new GitComment()
            {
                Content = comment.Content,
                Parent = new GitCommentParent() { Id = comment.Id },
                From = comment.From,
                To = comment.To,
                Path = comment.Path
            };

            var responseComment = await _gitClientService.AddPullRequestComment(Id, newComment);
            //todo this comment has to be added to comments list and update tree ^_^
        }

        private Task EditComment(GitComment comment)
        {
            throw new NotImplementedException();
        }

        private async Task DeleteComment(GitComment comment)
        {
            await _gitClientService.DeletePullRequestComment(Id, comment.Id, comment.Version);
            //todo this comment has to be added to removed from comments list and update tree ^_^
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
