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
        List<ITreeFile> FilesTree { get; }
        List<FileDiff> FileDiffs { get; }
        List<GitCommit> Commits { get; }
        string FromCommit { get; set; }
        string ToCommit { get; set; }
        ICommentViewModel CommentViewModel { get; }

        void AddFileDiffs(IEnumerable<FileDiff> fileDiffs);
        void AddCommits(IEnumerable<GitCommit> commits);
        void AddComments(long pullRequestId, IEnumerable<GitComment> pqComments);
    }
}