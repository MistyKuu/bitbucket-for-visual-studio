using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Contracts.Models.Tree;
using ReactiveUI;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(ICommentViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CommentViewModel : ViewModelBase, ICommentViewModel
    {
        private List<ICommentTree> _commentTree;
        private List<ICommentTree> _inlineCommentTree;
        private int _commentsCount;
        private readonly IGitClientService _gitClientService;
        private readonly ITreeStructureGenerator _treeStructureGenerator;

        public ReactiveCommand ReplyCommentCommand { get; private set; }
        public ReactiveCommand EditCommentCommand { get; private set; }
        public ReactiveCommand DeleteCommentCommand { get; private set; }
        public ReactiveCommand AddCommentCommand { get; private set; }

        public long PullRequestId { get; private set; }

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

        [ImportingConstructor]
        public CommentViewModel(IGitClientService gitClientService, ITreeStructureGenerator treeStructureGenerator)
        {
            _gitClientService = gitClientService;
            _treeStructureGenerator = treeStructureGenerator;
        }

        public void AddComments(long pullRequestId, IEnumerable<GitComment> pqComments)
        {
            PullRequestId = pullRequestId;
            var inlineComments = pqComments.Where(comment => comment.IsInline).ToList();
            var notInlineComments = pqComments.Where(x => !x.IsInline).ToList();

            InlineCommentTree = _treeStructureGenerator.CreateCommentTree(inlineComments).ToList();
            CommentTree = _treeStructureGenerator.CreateCommentTree(notInlineComments).ToList();
            CommentsCount = notInlineComments.Count(x => !x.IsDeleted);
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

            await _gitClientService.AddPullRequestComment(PullRequestId, newComment);
            var comments = await _gitClientService.GetPullRequestComments(PullRequestId);
            AddComments(PullRequestId, comments);//todo temp solution just to make it work
        }

        private Task EditComment(ICommentTree commentTree)
        {
            throw new NotImplementedException();
        }

        private Task<ICommentTree> AddComment(ICommentTree commentTree)
        {
            throw new NotImplementedException();
        }

        private async Task DeleteComment(ICommentTree commentTree)
        {
            var comment = commentTree.Comment;
            await _gitClientService.DeletePullRequestComment(PullRequestId, comment.Id, comment.Version);
            var comments = await _gitClientService.GetPullRequestComments(PullRequestId);
            AddComments(PullRequestId, comments);
        }

        public void InitializeCommands()
        {
            ReplyCommentCommand = ReactiveCommand.CreateFromTask<ICommentTree>(ReplyToComment);
            EditCommentCommand = ReactiveCommand.CreateFromTask<ICommentTree>(EditComment);
            DeleteCommentCommand = ReactiveCommand.CreateFromTask<ICommentTree>(DeleteComment);
            AddCommentCommand = ReactiveCommand.CreateFromTask<ICommentTree>(AddComment);
        }


        public string ErrorMessage { get; set; }
        public IEnumerable<ReactiveCommand> ThrowableCommands => new[] { ReplyCommentCommand, EditCommentCommand, DeleteCommentCommand };
    }
}