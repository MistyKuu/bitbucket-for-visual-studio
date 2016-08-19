using System.Collections.Generic;

namespace GitClientVS.Contracts.Models.GitClientModels
{
    public class GitBranch
    {
        public GitCommit Target { get; set; }

        public string Name { get; set; }
    }
}