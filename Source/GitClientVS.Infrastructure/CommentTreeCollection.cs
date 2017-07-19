using System.Collections.Generic;
using System.Linq;
using GitClientVS.Contracts.Models.Tree;

namespace GitClientVS.Infrastructure
{
    public class CommentTreeCollection
    {
        public IEnumerable<ICommentTree> Elements { get; private set; }

        public CommentTreeCollection(IEnumerable<ICommentTree> commentTree)
        {
            Elements = commentTree.ToList();
        }
    }
}