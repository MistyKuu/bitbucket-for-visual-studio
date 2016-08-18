using System;

namespace BitBucket.REST.API.Models
{
    public class Connection
    {
        internal static readonly Uri DefaultBitbucketUrl = new Uri("https://api.bitbucket.org/2.0/");

        public Connection()
            : this(DefaultBitbucketUrl, new Credentials())
        {
            
        }

        public Connection(Credentials credentials)
            : this(DefaultBitbucketUrl, credentials)
        {
            
        }

        public Connection(Uri bitbucketUrl, Credentials credentials)
        {
            BitbucketUrl = bitbucketUrl;
            Credentials = credentials;

            // todo: check in custom server
   
        }

        public string GetBitbucketUrl()
        {
            return "https://bitbucket.org";
        }

        public string GetHost() { return "bitbucket.org"; }

        public Credentials Credentials { get; private set; }

        public Uri BitbucketUrl { get; private set; }
    }
}