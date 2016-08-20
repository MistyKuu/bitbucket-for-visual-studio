using System;
using System.Collections.Generic;
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
                return "Insufficient privileges";
            if (ex is RequestFailedException)
                return "Wrong Request";

            return "Unknown error";
        }
    }
}
