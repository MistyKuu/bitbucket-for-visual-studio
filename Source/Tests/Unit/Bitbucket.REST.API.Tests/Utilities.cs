using System;
using System.IO;

namespace Bitbucket.REST.API.Tests
{
    public static class Utilities
    {
        public static string LoadFile(string fileName)
        {
            return string.Join(Environment.NewLine, File.ReadAllLines(fileName));
        }
    }
}
