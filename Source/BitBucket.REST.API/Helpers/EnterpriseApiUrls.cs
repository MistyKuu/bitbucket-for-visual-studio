using System;
using System.Globalization;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.Helpers
{
    public static class EnterpriseApiUrls
    {
        public static string Repositories()
        {
            return $"repos";
        }

        public static string Teams()
        {
            return $"groups";
        }

        public static string CreateRepositories(string ownerName)
        {   
            return $"users/{ownerName}/repos";
        }

        public static string RepositoriesRecent()
        {
            return $"profile/recent/repos";
        }

        public static string Branches(string ownerName, string repoName)
        {
            return $"projects/{ownerName}/repos/{repoName}/branches";
        }

        public static string Users()
        {
            return $"users";
        }

        public static string DownloadFile(string ownerName, string repoName, string hash, string filePath)
        {
            return $"projects/{ownerName}/repos/{repoName}/browse/{filePath}?at={hash}";
        }

        public static string Commits(string ownerName, string repository)
        {
            return $"projects/{ownerName}/repos/{repository}/commits";
        }

        public static string Commit(string ownerName, string repository, string id)
        {
            return $"projects/{ownerName}/repos/{repository}/commits/{id}";
        }

        public static string CommitsDiff(string owner, string repoName)
        {
            return $"projects/{owner}/repos/{repoName}/compare/diff";
        }

        public static string PullRequests(string ownerName, string repository)
        {
            return $"projects/{ownerName}/repos/{repository}/pull-requests";
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
            return $"projects/{owner}/repos/{repositoryName}/pull-requests/{id}/diff";
        }

        public static string PullRequestApprove(string owner, string repositoryName, long id)
        {
            return $"projects/{owner}/repos/{repositoryName}/pull-requests/{id}/approve";
        }

        public static string PullRequestDecline(string owner, string repositoryName, long id, string version)
        {
            return $"projects/{owner}/repos/{repositoryName}/pull-requests/{id}/decline?version={version}";
        }

        public static string PullRequestMerge(string owner, string repositoryName, long id, string version)
        {
            return $"projects/{owner}/repos/{repositoryName}/pull-requests/{id}/merge?version={version}";
        }

        public static string PullRequestCommits(string owner, string repositoryName, long id)
        {
            return $"projects/{owner}/repos/{repositoryName}/pull-requests/{id}/commits";
        }

        public static string PullRequestActivities(string owner, string repositoryName, long id)
        {
            return $"projects/{owner}/repos/{repositoryName}/pull-requests/{id}/activities";
        }
        public static string PullRequestComments(string owner, string repositoryName, long id)
        {
            return $"projects/{owner}/repos/{repositoryName}/pull-requests/{id}/commits";
        }
    }
}