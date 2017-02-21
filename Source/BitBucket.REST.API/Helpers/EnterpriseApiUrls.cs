using System;
using System.Globalization;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.Helpers
{
    public static class EnterpriseApiUrls
    {
        public static string Repositories(string ownerName)
        {
            return $"repos";
        }

        public static string Branches(string ownerName, string repoName)
        {
            return $"projects/{ownerName}/repos/{repoName}/branches";
        }

        public static string Commits(string ownerName, string repoName, string id)
        {
            return $"projects/{ownerName}/repos/{repoName}/commits/{id}";
        }

        public static string User()
        {
            return "users";
        }

        public static string PullRequests(string ownerName, string repository)
        {
            return $"projects/{ownerName}/repos/{repository}/pull-requests?state=ALL";
        }
        public static string PullRequest(string ownerName, string repository, long id)
        {
            return $"projects/{ownerName}/repos/{repository}/pull-requests/{id}";
        }
        public static string PullRequestsAuthors(string ownerName, string repositoryName)
        {
            return $"projects/{ownerName}/repos/{repositoryName}/participants";
        }

        public static string PullRequestDiff(string owner, string repositoryName, long id)
        {
            return $"projects/{owner}/repos/{repositoryName}/pull-requests/{id}/diff?CONTEXTLINES&SRCPATH&WHITESPACE&WITHCOMMENTS";
        }

        public static string PullRequestApprove(string owner, string repositoryName, long id)
        {
            return $"projects/{owner}/repos/{repositoryName}/pull-requests/{id}/approve";
        }

        public static string PullRequestCommits(string owner, string repositoryName, long id)
        {
            return $"projects/{owner}/repos/{repositoryName}/pull-requests/{id}/commits";
        }

        public static string PullRequestActivities(string owner, string repositoryName, long id)
        {
            return $"projects/{owner}/repos/{repositoryName}/pull-requests/{id}/activities";
        }
    }
}