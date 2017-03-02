using System.Collections.Generic;

namespace GitClientVS.Contracts.Models.GitClientModels
{
    public class GitBranch
    {
        public GitCommit Target { get; set; }

        public string Name { get; set; }

        public bool IsRemote { get; set; }

        public bool IsHead { get; set; }

        public string TrackedBranchName { get; set; }
    }
}