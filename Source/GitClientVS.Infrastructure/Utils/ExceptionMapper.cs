using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitBucket.REST.API.Exceptions;

namespace GitClientVS.Infrastructure.Utils
{
    public static class ExceptionMapper
    {
        public static string Map(Exception ex)
        {
            if (ex is AuthorizationException)
                return "Incorrect credentials";
            if (ex is ForbiddenException)
                return "Operation is Forbidden";
            if (ex is RequestFailedException)
            {
                var failedEx = ((RequestFailedException)ex);
                return failedEx.IsFriendlyMessage ? ex.Message : "Wrong request";
            }

            if (ex is UnauthorizedAccessException)
                return "Unauthorized";

            return $"Unknown error ({ex.GetType()}). Check logs for more info";
        }
    }
}
