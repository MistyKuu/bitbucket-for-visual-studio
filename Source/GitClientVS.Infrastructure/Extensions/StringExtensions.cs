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
      

        public static bool Contains(this string source, string subString, StringComparison stringComparison)
        {
            return source.IndexOf(subString, stringComparison) >= 0;
        }
    }
}
