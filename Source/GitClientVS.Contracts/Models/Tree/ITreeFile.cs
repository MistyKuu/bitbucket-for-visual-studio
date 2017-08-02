using System;
using System.Collections.Generic;
using ParseDiff;

namespace GitClientVS.Contracts.Models.Tree
{
    public interface ITreeFile
    {
        string Name { get; set; }
        List<ITreeFile> Files { get; set; }
        FileDiff FileDiff { get; set; }
        bool IsAdded { get; set; }
        bool IsRemoved { get; set; }
        long Added { get; set; }
        long Removed { get; set; }
        long Comments { get; set; }
        bool IsExpanded { get; set; }

        bool IsSelectable { get; set; }
        Type GetTreeType { get; }
    }
}