namespace BitBucket.REST.API.Exceptions
{
    public class AuthorizationException : AppException
    {
        public AuthorizationException()
        {
            DisplayedMessage = "Unauthorized / Invalid Credentials";
        }
    }
}