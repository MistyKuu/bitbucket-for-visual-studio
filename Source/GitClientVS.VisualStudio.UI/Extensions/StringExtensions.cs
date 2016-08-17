using System;

namespace GitClientVS.VisualStudio.UI.Extensions
{
    public static class StringExtensions
    {
        public static string TrimEnd(this string s, string suffix)
        {
            if (s == null) return null;
            if (!s.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                return s;

            return s.Substring(0, s.Length - suffix.Length);
        }
    }
}