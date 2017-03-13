using System;
using System.Globalization;

namespace BitBucket.REST.API.Helpers
{
    public static class ApiUrls
    {
        public static Uri FormatUri(this string pattern, params object[] args)
        {
            return new Uri(string.Format(CultureInfo.InvariantCulture, pattern, args), UriKind.Relative);
        }

        public static string Repositories(string login)
        {
            return $"repositories/{login}";
        }

        public static string Teams(string role = "member")
        {
            return $"teams?role={role}";
        }

        public static string Branches(string login, string repoName)
        {
            return $"repositories/{login}/{repoName}/refs/branches";
        }

        public static string Repository(string login, string repoName)
        {
            return $"repositories/{login}/{repoName}";
        }

        public static string PullRequests(string username, string repository)
        {
            return $"repositories/{username}/{repository}/pullrequests?state=ALL";
        }

        public static string PullRequestsAuthors(string username, string repository)
        {
            return $"repositories/{username}/{repository}/pr-authors/";
        }

        public static string RepositoryUsers(string username, string repository)
        {
            return $"privileges/{username}/{repository}";
        }

        public static string PullRequest(string username, string repository, long id)
        {
            return $"repositories/{username}/{repository}/pullrequests/{id}";
        }

        public static string PullRequestApprove(string username, string repository, long id)
        {
            return $"repositories/{username}/{repository}/pullrequests/{id}/approve";
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

        public static string User()
        {
            return "user";
        }

        public static string Repositories()
        {
            return "repositories";
        }

    }
}