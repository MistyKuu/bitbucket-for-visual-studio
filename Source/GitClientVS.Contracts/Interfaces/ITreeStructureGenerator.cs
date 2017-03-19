using System.Collections.Generic;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using ParseDiff;

namespace GitClientVS.Contracts.Interfaces
{
    public interface ITreeStructureGenerator
    {
        IEnumerable<ICommentTree> CreateCommentTree(List<GitComment> gitComments, Theme currentTheme, char separator = '/');
        IEnumerable<ITreeFile> CreateFileTree(List<FileDiff> fileDiffs, string rootFileName = "test", char separator = '/');
    }
}