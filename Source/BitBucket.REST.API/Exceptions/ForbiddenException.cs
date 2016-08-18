using System;

namespace BitBucket.REST.API.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message)
        {

        }


    }
}