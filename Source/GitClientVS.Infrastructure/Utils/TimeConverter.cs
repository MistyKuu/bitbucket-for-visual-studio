using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Infrastructure.Utils
{
    public static class TimeConverter
    {
        public static DateTime GetDate(string date) // TODO this needs to take into account current culture
        {
            return DateTime.Parse(date);
        }
    }
}
