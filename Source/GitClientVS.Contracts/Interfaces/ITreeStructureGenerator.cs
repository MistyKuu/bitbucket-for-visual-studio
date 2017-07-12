using System.Collections.Generic;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Contracts.Models.Tree;
using ParseDiff;

namespace GitClientVS.Contracts.Interfaces
{
    public interface ITreeStructureGenerator
    {
        IEnumerable<ICommentTree> CreateCommentTree(IEnumerable<GitComment> gitComments, char separator = '/');
        IEnumerable<ITreeFile> CreateFileTree(IEnumerable<FileDiff> fileDiffs, string rootFileName = "test", char separator = '/');
    }
}