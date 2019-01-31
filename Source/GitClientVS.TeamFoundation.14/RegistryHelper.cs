using LibGit2Sharp;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GitClientVS.TeamFoundation
{
    internal class RegistryHelper
    {
        private static int MajorVersion => Process.GetCurrentProcess().MainModule.FileVersionInfo.FileMajorPart;
        private static RegistryKey OpenGitKey(string path)
        {
            var keyName = $"Software\\Microsoft\\VisualStudio\\{MajorVersion}.0\\TeamFoundation\\GitSourceControl\\{path}";
            return Registry.CurrentUser.OpenSubKey(keyName, true);
        }

        public static string GetLocalClonePath()
        {
            using (var key = OpenGitKey("General"))
            {
                return (string)key?.GetValue("DefaultRepositoryPath", string.Empty, RegistryValueOptions.DoNotExpandEnvironmentNames);
            }
        }

        public static IEnumerable<string> GetLocalRepositories()
        {
            using (var key = OpenGitKey("Repositories"))
            {
                return key.GetSubKeyNames().Select(x =>
                {
                    using (var subkey = key.OpenSubKey(x))
                    {
                        try
                        {
                            if (subkey?.GetValue("Path") is string path)
                                return path;
                        }
                        catch (Exception)
                        {
                        }

                        return null;
                    }
                })
                .Where(x => x != null)
                .ToList();
            }
        }
    }
}
