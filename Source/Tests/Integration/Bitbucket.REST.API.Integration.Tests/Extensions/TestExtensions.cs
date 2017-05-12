using System;
using System.Threading.Tasks;

namespace Bitbucket.REST.API.Integration.Tests.Extensions
{
    public static class TestExtensions
    {
        public static Task<T> FromTaskAsync<T>(this T obj)
        {
            return Task.FromResult(obj);
        }
    }
}
