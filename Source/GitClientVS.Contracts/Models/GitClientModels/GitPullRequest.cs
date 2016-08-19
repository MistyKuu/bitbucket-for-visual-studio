using System;
using System.Collections.Generic;

namespace GitClientVS.Contracts.Models.GitClientModels
{
    public class GitPullRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string SourceBranch { get; set; }
        public string DestinationBranch { get; set; }
        public GitPullRequestStatus Status { get; set; }
        public long Id { get; set; }
        public GitUser Author { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool? CloseSourceBranch { get; set; }
        public string Url { get; set; }
        public Dictionary<string, bool> Reviewers { get; set; }
        public string Link { get; set; }
        public int CommentsCount { get; set; }

        public GitPullRequest(string title, string description, string sourceBranch, string destinationBranch) : 
            this(title, description, sourceBranch, destinationBranch, null)
        {
        }

        public GitPullRequest(string title, string description, string sourceBranch, string destinationBranch, Dictionary<string, bool> reviewers)
        {
            Title = title;
            Description = description;
            SourceBranch = sourceBranch;
            DestinationBranch = destinationBranch;
            Reviewers = reviewers;
        }
    }
}