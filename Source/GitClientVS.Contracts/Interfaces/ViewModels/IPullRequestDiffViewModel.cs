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
        List<ITreeFile> FilesTree { get; set; }
        List<FileDiff> FileDiffs { get; set; }
        List<GitCommit> Commits { get; set; }
        List<ICommentTree> CommentTree { get; set; }
        int CommentsCount { get; }
        string FromCommit { get; set; }
        string ToCommit { get; set; }
        List<ICommentTree> InlineCommentTree { get; set; }
        long Id { get; set; }
    }
}