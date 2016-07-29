using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Contracts.Models.GitClientModels
{
    public class GitUser
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public GitLinks Links { get; set; }
    }
}
