using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Infrastructure
{
    public static class Paths
    {
        public static string GitClientStorageDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"GitClientVSExtension");
        public static string GitClientLogFilePath => Path.Combine(GitClientStorageDirectory, "Logs", "logs.txt");
        public static string GitClientUserDataPath => Path.Combine(GitClientStorageDirectory, "User", "data.dat");
    }
}
