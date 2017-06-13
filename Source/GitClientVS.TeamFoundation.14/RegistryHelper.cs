using LibGit2Sharp;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GitClientVS.TeamFoundation
{
    internal class RegistryHelper
    {
        const string TEGitKey = @"Software\Microsoft\VisualStudio\14.0\TeamFoundation\GitSourceControl";
        private static RegistryKey OpenGitKey(string path)
        {
            return Registry.CurrentUser.OpenSubKey(TEGitKey + "\\" + path, true);
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
