namespace GitClientVS.Contracts.Models.GitClientModels
{
    public class GitLocalBranch : GitBranch
    {
        public bool IsRemote { get; set; }

        public bool IsHead { get; set; }

        public string TrackedBranchName { get; set; }
    }
}