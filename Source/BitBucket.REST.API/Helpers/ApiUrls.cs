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

        public static string CreateRepository(string login, string repoName)
        {
            return $"repositories/{login}/{repoName}";
        }

        public static string PullRequests(string username, string repository)
        {
            return $"repositories/{username}/{repository}/pullrequests";
        }

        public static string PullRequest(string username, string repository, long id)
        {
            return $"repositories/{username}/{repository}/pullrequests/{id}";
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