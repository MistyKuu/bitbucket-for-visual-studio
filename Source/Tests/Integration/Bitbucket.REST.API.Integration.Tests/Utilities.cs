using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitbucket.REST.API.Integration.Tests
{
    public static class Utilities
    {
        public static string LoadFile(string fileName)
        {
            return string.Join(Environment.NewLine, File.ReadAllLines(fileName));
        }
    }
}
