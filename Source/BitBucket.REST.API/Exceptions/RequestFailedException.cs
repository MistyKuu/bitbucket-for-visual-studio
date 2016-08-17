using System;

namespace BitBucket.REST.API.Exceptions
{
    public class RequestFailedException : Exception
    {
        public RequestFailedException(string message) : base(message)
        {
            
        }


    }
}