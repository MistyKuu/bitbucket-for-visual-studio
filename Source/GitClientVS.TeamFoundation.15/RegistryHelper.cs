using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.Win32;

namespace GitClientVS.TeamFoundation
{
    internal class RegistryHelper
    {
        const string TEGitKey = @"Software\Microsoft\VisualStudio\15.0\TeamFoundation\GitSourceControl";
        static RegistryKey OpenGitKey(string path)
        {
            return Registry.CurrentUser.OpenSubKey(TEGitKey + "\\" + path, true);
        }

        internal static string GetLocalClonePath()
        {
            using (var key = OpenGitKey("General"))
            {
                return (string)key?.GetValue("DefaultRepositoryPath", string.Empty, RegistryValueOptions.DoNotExpandEnvironmentNames);
            }
        }
    }
}
