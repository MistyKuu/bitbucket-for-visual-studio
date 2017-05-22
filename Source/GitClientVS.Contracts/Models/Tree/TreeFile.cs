using System;
using System.Collections.Generic;
using ParseDiff;

namespace GitClientVS.Contracts.Models.Tree
{
    public class TreeFile: ITreeFile
    {
        public string Name { get; set; }
        public List<ITreeFile> Files { get; set; }
        public bool IsSelected { get; set; }
        public FileDiff FileDiff { get; set; }
        public bool IsAdded { get; set; }
        public bool IsRemoved { get; set; }
        public Type GetTreeType { get { return this.GetType(); } }
        public bool IsExpanded { get; set; }
        public bool IsSelectable { get; set; }
        public long Added { get; set; }
        public long Removed { get; set; }

        public TreeFile(string name) : this(name, null)
        {
        }

        public TreeFile(string name, FileDiff fileDiff)
        {
            Name = name;
            Files = new List<ITreeFile>();
            FileDiff = fileDiff;
            IsSelectable = true;
            IsAdded = fileDiff.Add;
            IsRemoved = fileDiff.Deleted;
            Added = fileDiff.Additions;
            Removed = fileDiff.Deletions;
        }
    }
}