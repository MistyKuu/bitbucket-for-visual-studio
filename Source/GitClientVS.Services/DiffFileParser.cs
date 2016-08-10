using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParseDiff;

namespace GitClientVS.Services
{
    public class DiffFileParser
    {
        public void Parse(string diff)
        {
            Diff.Parse(diff);
        }
    }
}
