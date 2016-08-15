using System.Collections.Generic;
using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Contracts.Models
{
    public interface ICommentTree
    {
        List<ICommentTree> Comments { get; set; }
        GitComment Comment { get; set; }
        bool IsExpanded { get; set; }
    }
}