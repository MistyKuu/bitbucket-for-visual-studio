using System;

namespace BitBucket.REST.API.Models
{
    public class Connection
    {
        public Connection(Uri apiUrl, Credentials credentials)
        {
            ApiUrl = apiUrl;
            Credentials = credentials;
        }

        public Uri GetBitbucketUrl()
        {
            return ApiUrl;
        }

        public string GetHost()
        {
            return GetBitbucketUrl().Host;
        }

        public Credentials Credentials { get; private set; }

        public Uri ApiUrl { get; private set; }
    }
}