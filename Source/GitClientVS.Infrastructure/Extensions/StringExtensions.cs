using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static SecureString ToSecureString(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;
            else
            {
                SecureString result = new SecureString();
                foreach (char c in source)
                    result.AppendChar(c);
                return result;
            }
        }

        public static bool Contains(this string source, string subString, StringComparison stringComparison)
        {
            return source.IndexOf(subString, stringComparison) >= 0;
        }
    }
}
