using System.Collections.Generic;
using ParseDiff;

namespace GitClientVS.Contracts.Models
{
    public class TreeFile: ITreeFile
    {
        public string Name { get; set; }
        public List<ITreeFile> Files { get; set; }
        public bool IsSelected { get; set; }
        public FileDiff FileDiff { get; set; }

        public TreeFile(string name) : this(name, null)
        {
        }

        public TreeFile(string name, FileDiff fileDiff)
        {
            Name = name;
            Files = new List<ITreeFile>();
            FileDiff = fileDiff;
        }
    }
}