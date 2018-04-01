using System;

namespace BitBucket.REST.API.Exceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message) : base(message)
        {
            DisplayedMessage = "Operation is forbidden";
        }
    }
}