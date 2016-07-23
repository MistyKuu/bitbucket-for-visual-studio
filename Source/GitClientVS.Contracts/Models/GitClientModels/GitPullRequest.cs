namespace GitClientVS.Contracts.Models.GitClientModels
{
    public class GitPullRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string SourceBranch { get; set; }
        public string DestinationBranch { get; set; }
        public string RepoName { get; set; }
        public string Id { get; set; }
        public string Author { get; set; }

        public GitPullRequest(string title, string description, string sourceBranch, string destinationBranch,
            string repoName)
        {
            Title = title;
            Description = description;
            SourceBranch = sourceBranch;
            DestinationBranch = destinationBranch;
            RepoName = repoName;
        }
    }
}