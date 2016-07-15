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
            return String.Format("repositories/{0}", login);
        }

        public static string Repositories()
        {
            return String.Format("repositories");
        }

    }
}