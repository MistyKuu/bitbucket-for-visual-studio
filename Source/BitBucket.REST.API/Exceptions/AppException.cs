using System;

namespace BitBucket.REST.API.Exceptions
{
    public class AppException : Exception
    {
        public AppException()
        {

        }

        public AppException(string message) : base(message)
        {

        }

        public string DisplayedMessage { get; set; }
    }
}