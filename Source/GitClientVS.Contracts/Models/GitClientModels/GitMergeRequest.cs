using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GitClientVS.Contracts.Models.GitClientModels
{
    public class GitMergeRequest
    {
        public long Id { get; set; }
        public bool? CloseSourceBranch { get; set; }
        public string MergeStrategy { get; set; } //merge_commit, squash, todo enum
        public string Version { get; set; }
    }
}
