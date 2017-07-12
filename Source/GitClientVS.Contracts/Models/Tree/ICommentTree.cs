using System.Collections.Generic;
using System.Collections.ObjectModel;
using GitClientVS.Contracts.Models.GitClientModels;
using ReactiveUI;

namespace GitClientVS.Contracts.Models.Tree
{
    public interface ICommentTree
    {
        ReactiveList<ICommentTree> Comments { get; set; }
        GitComment Comment { get; set; }
        bool IsExpanded { get; set; }
        bool AllDeleted { get; }
        void DeleteCurrentComment();
        void AddComment(GitComment comment);
    }
}