using System;

namespace BitBucket.REST.API.Exceptions
{
    public class AuthorizationException : Exception
    {
     

        public override string Message
        {
            get { return "Unauthorized"; }
        }
    }
}