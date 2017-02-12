using System;
using System.Globalization;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.Helpers
{
    public static class EnterpriseApiUrls
    {
        public static string Repositories(string login)
        {
            return $"projects/~{login}/repos";
        }

        public static string Branches(string login, string repoName)
        {
            return $"projects/~{login}/repos/{repoName}/branches";
        }

        public static string Commits(string login, string repoName)
        {
            return $"projects/~{login}/repos/{repoName}/commits";
        }

        public static string User()
        {
            return "users";
        }

        public static string PullRequests(string login, string repository)
        {
            return $"projects/~{login}/repos/{repository}/pull-requests?state=ALL";
        }
        public static string PullRequest(string login, string repository, long id)
        {
            return $"projects/~{login}/repos/{repository}/pull-requests/{id}";
        }
        public static string PullRequestsAuthors(string ownerName, string repositoryName)
        {
            return $"projects/~{ownerName}/repos/{repositoryName}/participants";
        }

        public static string PullRequestDiff(string owner, string repositoryName, long id)
        {
            return $"projects/~{owner}/repos/{repositoryName}/pull-requests/{id}/diff?CONTEXTLINES&SRCPATH&WHITESPACE&WITHCOMMENTS";
        }

        public static string PullRequestApprove(string owner, string repositoryName, long id)
        {
            return $"projects/~{owner}/repos/{repositoryName}/pull-requests/{id}/participants/{owner}";
        }

        public static string PullRequestCommits(string owner, string repositoryName, long id)
        {
            return $"projects/~{owner}/repos/{repositoryName}/pull-requests/{id}/commits";
        }

        public static string PullRequestActivities(string owner, string repositoryName, long id)
        {
            return $"projects/~{owner}/repos/{repositoryName}/pull-requests/{id}/activities";
        }
    }
}