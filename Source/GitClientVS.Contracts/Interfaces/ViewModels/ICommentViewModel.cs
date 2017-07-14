using System.Collections.Generic;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Contracts.Models.Tree;
using ReactiveUI;

namespace GitClientVS.Contracts.Interfaces.ViewModels
{
    public interface ICommentViewModel : IViewModelWithErrorMessage
    {
        ReactiveCommand ReplyCommentCommand { get; }
        ReactiveCommand EditCommentCommand { get; }
        ReactiveCommand DeleteCommentCommand { get; }
        ReactiveCommand AddCommentCommand { get; }

        List<ICommentTree> CommentTree { get; }
        int CommentsCount { get; }
        long PullRequestId { get; }
        List<ICommentTree> InlineCommentTree { get; }

        void AddComments(long pullRequestId, IEnumerable<GitComment> pqComments);
    }
}