using System.Collections.Generic;
using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Contracts.Models.Tree
{
    public class CommentTree : ICommentTree
    {
        public List<ICommentTree> Comments { get; set; }
        public GitComment Comment { get; set; }
        public bool IsExpanded { get; set; }

        public CommentTree()
        {
            Comments = new List<ICommentTree>();
        }

        public CommentTree(GitComment comment)
        {
            Comment = comment;
            Comments = new List<ICommentTree>();
            IsExpanded = true;
        }
    }
}