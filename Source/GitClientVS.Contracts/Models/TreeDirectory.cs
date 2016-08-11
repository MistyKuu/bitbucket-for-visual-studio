using System.Collections.Generic;
using ParseDiff;

namespace GitClientVS.Contracts.Models
{
    public class TreeDirectory: ITreeFile
    {
        public string Name { get; set; }
        public List<ITreeFile> Files { get; set; }
        public bool IsSelected { get; set; }
        public FileDiff FileDiff { get; set; }

        public TreeDirectory(string name)
        {
            Name = name;
            Files = new List<ITreeFile>();
            FileDiff = null;
        }
    }
}