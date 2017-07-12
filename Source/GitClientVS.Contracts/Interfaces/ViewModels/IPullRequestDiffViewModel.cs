using System.Collections.Generic;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Contracts.Models.Tree;
using ParseDiff;
using ReactiveUI;

namespace GitClientVS.Contracts.Interfaces.ViewModels
{
    public interface IPullRequestDiffViewModel : IViewModelWithErrorMessage
    {
        ReactiveCommand ShowDiffCommand { get; }
        ReactiveCommand ReplyCommentCommand { get; }
        ReactiveCommand EditCommentCommand { get; }
        ReactiveCommand DeleteCommentCommand { get; }
        List<ITreeFile> FilesTree { get;}
        List<FileDiff> FileDiffs { get; }
        List<GitCommit> Commits { get; }
        List<ICommentTree> CommentTree { get;  }
        int CommentsCount { get; }
        List<ICommentTree> InlineCommentTree { get; }
        long Id { get; set; }
        string FromCommit { get; set; }
        string ToCommit { get; set; }

        void AddFileDiffs(IEnumerable<FileDiff> fileDiffs);
        void AddComments(IEnumerable<GitComment> pqComments);
        void AddCommits(IEnumerable<GitCommit> commits);
    }
}