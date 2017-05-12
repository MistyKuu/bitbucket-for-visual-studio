using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Bitbucket.REST.API.Integration.Tests
{
    public static class Paths
    {
        public static string GetEnterpriseDataPath(string fileName)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, "Enterprise", "Data", fileName);
        }
    }
}
