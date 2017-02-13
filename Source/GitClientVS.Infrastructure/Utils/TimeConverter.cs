using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Infrastructure.Utils
{
    public static class TimeConverter
    {
        public static DateTime GetDate(string date)
        {
            return DateTime.Parse(date, CultureInfo.InvariantCulture).ToLocalTime();
        }
    }
}
