using System.Collections.Generic;
using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Contracts.Models
{
    public class CommentTree : ICommentTree
    {
        public List<ICommentTree> Comments { get; set; }
        public GitComment Comment { get; set; }
        public string Content { get; set; }

        public CommentTree()
        {
            Comments = new List<ICommentTree>();
        }

        public CommentTree(GitComment comment)
        {
            Comment = comment;
            Comments = new List<ICommentTree>();
        }

        public CommentTree(string content)
        {
            Content = content;
            Comments = new List<ICommentTree>();
        }
    }
}