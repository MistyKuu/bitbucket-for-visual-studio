﻿using System;
using System.Globalization;

namespace BitBucket.REST.API.Helpers
{
    public static class ApiUrls
    {
        public static Uri FormatUri(this string pattern, params object[] args)
        {
            return new Uri(string.Format(CultureInfo.InvariantCulture, pattern, args), UriKind.Relative);
        }

        public static string Repositories()
        {
            return $"repositories";
        }

        public static string Commits(string ownerName, string repoName)
        {
            return $"repositories/{ownerName}/{repoName}/commits";
        }

        public static string CommitsDiff(string ownerName, string repoName, string fromHash, string toHash)
        {
            return $"repositories/{ownerName}/{repoName}/diff/{fromHash}..{toHash}";
        }

        public static string Commits(string ownerName, string repoName, string branch)
        {
            return $"repositories/{ownerName}/{repoName}/commits/{branch}";
        }

        public static string Commit(string ownerName, string repoName, string id)
        {
            return $"repositories/{ownerName}/{repoName}/commit/{id}";
        }

        public static string DownloadFile(string ownerName, string repoName, string hash, string filePath)
        {
            return $"repositories/{ownerName}/{repoName}/src/{hash}/{filePath}";
        }

        public static string Teams(string role = "member")
        {
            return $"teams?role={role}";
        }
        public static string Branches(string login, string repoName)
        {
            return $"repositories/{login}/{repoName}/refs/branches";
        }

        public static string DefaultBranch(string login, string repoName)
        {
            return $"repositories/{login}/{repoName}/main-branch";
        }

        public static string Repository(string login, string repoName)
        {
            return $"repositories/{login}/{repoName}";
        }

        public static string PullRequests(string username, string repository)
        {
            return $"repositories/{username}/{repository}/pullrequests";
        }

        public static string RepositoryUsers(string username, string repository)
        {
            return $"teams/{username}/permissions/repositories/{repository}";
        }

        public static string PullRequest(string username, string repository, long id)
        {
            return $"repositories/{username}/{repository}/pullrequests/{id}";
        }

        public static string PullRequestApprove(string username, string repository, long id)
        {
            return $"repositories/{username}/{repository}/pullrequests/{id}/approve";
        }

        public static string PullRequestDecline(string username, string repository, long id)
        {
            return $"repositories/{username}/{repository}/pullrequests/{id}/decline";
        }

        public static string PullRequestMerge(string username, string repository, long id)
        {
            return $"repositories/{username}/{repository}/pullrequests/{id}/merge";
        }

        public static string PullRequestDiff(string username, string repository, long id)
        {
            return $"repositories/{username}/{repository}/pullrequests/{id}/diff";
        }

        public static string PullRequestCommits(string username, string repository, long id)
        {
            return $"repositories/{username}/{repository}/pullrequests/{id}/commits";
        }

        public static string PullRequestComments(string username, string repository, long id)
        {
            return $"repositories/{username}/{repository}/pullrequests/{id}/comments";
        }
        public static string PullRequestComments(string username, string repository,long pullRequestId, long commentId)
        {
            return $"repositories/{username}/{repository}/pullrequests/{pullRequestId}/comments/{commentId}";
        }

        public static string User()
        {
            return "user";
        }
        public static string User(string id)
        {
            return $"user/{id}";
        }


        public static string DefaultReviewers(string username, string repository)
        {
            return $"repositories/{username}/{repository}/default-reviewers";
        }
    }
}