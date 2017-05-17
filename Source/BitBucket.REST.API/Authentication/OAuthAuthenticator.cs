using BitBucket.REST.API.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace BitBucket.REST.API.Authentication
{
    public class OAuthAuthenticator
    {
        private const string TokenUrl = "https://bitbucket.org/site/oauth2/access_token";
        private const string TokenType = "Bearer";

        public OAuthAuthenticator(Credentials credentials)
        {
        }
    }
}