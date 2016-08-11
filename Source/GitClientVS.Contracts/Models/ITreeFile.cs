using System.Collections.Generic;
using ParseDiff;

namespace GitClientVS.Contracts.Models
{
    public interface ITreeFile
    {
        string Name { get; set; }
        List<ITreeFile> Files { get; set; }
        bool IsSelected { get; set; }
        FileDiff FileDiff { get; set; }
    }
}