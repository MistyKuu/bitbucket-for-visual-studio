using System;
using System.Collections.Generic;
using ParseDiff;

namespace GitClientVS.Contracts.Models
{
    public class TreeDirectory: ITreeFile
    {
        public string Name { get; set; }
        public List<ITreeFile> Files { get; set; }
        public bool IsAdded { get; set; }
        public bool IsRemoved { get; set; }
        public FileDiff FileDiff { get; set; }
        public Type GetTreeType { get { return this.GetType(); } }
        public bool IsSelectable { get; set; }
        public long Added { get; set; }
        public long Removed { get; set; }

        public TreeDirectory(string name)
        {
            Name = name;
            Files = new List<ITreeFile>();
            FileDiff = null;
            IsSelectable = false;
        }
    }
}