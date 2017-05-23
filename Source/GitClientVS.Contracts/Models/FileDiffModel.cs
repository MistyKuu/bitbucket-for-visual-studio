using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Models.Tree;
using ParseDiff;

namespace GitClientVS.Contracts.Models
{
    public class FileDiffModel
    {
        public ITreeFile TreeFile { get; set; }
        public string FromCommit { get; set; }
        public string ToCommit { get; set; }
    }
}
