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
        public bool? HasIssues { get; set; }
        public bool? HasWiki { get; set; }
        public bool? IsForked { get; set; }
        public string CloneUrl { get; set; }

        public GitRemoteRepository()
        {
            
        }

        public GitRemoteRepository(string name, string cloneUrl)
        {
            Name = name;
            CloneUrl = cloneUrl;
        }
    }
}
