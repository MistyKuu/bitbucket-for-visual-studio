using System;

namespace BitBucket.REST.API.Exceptions
{
    public class RequestFailedException : AppException
    {
        public bool IsFriendlyMessage { get; set; }

        public RequestFailedException(string message, bool isFriendlyMessage) : base(message)
        {
            IsFriendlyMessage = isFriendlyMessage;
            DisplayedMessage = IsFriendlyMessage ? message : "Wrong request";
        }
    }
}