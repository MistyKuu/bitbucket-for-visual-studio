using RestSharp;
using RestSharp.Authenticators;

namespace BitBucket.REST.API.Authentication
{
    class AnonymousAuthenticator : IAuthenticator
    {
        public void Authenticate(IRestClient client, IRestRequest request)
        {
            
        }
    }
}