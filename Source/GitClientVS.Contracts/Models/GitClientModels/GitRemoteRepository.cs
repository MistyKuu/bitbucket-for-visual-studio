using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Contracts.Models.GitClientModels
{
    public class  GitRemoteRepository
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsPrivate { get; set; }
        public string FullName { get; set; }
        public GitUser Owner { get; set; }
        public GitLinks Links { get; set; }
    }
}
