using System.IO;
using NUnit.Framework;

namespace Bitbucket.REST.API.Tests
{
    public static class Paths
    {
        public static string GetEnterpriseDataPath(string fileName)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, "Enterprise", "Data", fileName);
        }
    }
}
