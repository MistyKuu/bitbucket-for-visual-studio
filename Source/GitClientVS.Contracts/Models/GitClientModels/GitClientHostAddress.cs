using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Contracts.Models.GitClientModels
{
    public class GitClientHostAddress
    {
        public Uri WebUri { get; set; }
        public Uri ApiUri { get; set; }

        public GitClientHostAddress()
        {
            WebUri = new Uri("https://bitbucket.org");
            ApiUri = new Uri("https://api.bitbucket.org/2.0");
        }

        public GitClientHostAddress(string web, string api)
        {
            WebUri = new Uri(web);
            ApiUri = new Uri(api);
        }
    }
}
