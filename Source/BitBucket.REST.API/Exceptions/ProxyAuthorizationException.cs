using System;

namespace BitBucket.REST.API.Exceptions
{
    public class ProxyAuthorizationException : AppException
    {
        public ProxyAuthorizationException(string message) : base(message)
        {
            DisplayedMessage = "Unauthorized Proxy";
        }
    }
}