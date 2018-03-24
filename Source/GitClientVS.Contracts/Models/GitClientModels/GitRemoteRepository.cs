using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Contracts.Models.GitClientModels
{
    //todo this represents both local, and remote. Name is unfortunate. change it later
    public class  GitRemoteRepository
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsPrivate { get; set; }
        public bool? HasIssues { get; set; }
        public bool? HasWiki { get; set; }
        public bool? IsForked { get; set; }
        public string Owner { get; set; }
        public string CloneUrl { get; set; }
        public List<GitLocalBranch> Branches { get; set; }
        public bool IsTeam { get; set; }

        public GitRemoteRepository()
        {
            
        }

        public GitRemoteRepository(string name, string owner, string cloneUrl, List<GitLocalBranch> branches)
        {
            Name = name.ToLowerInvariant();
            Owner = owner;
            CloneUrl = cloneUrl;
            Branches = branches;
        }
    }
}
