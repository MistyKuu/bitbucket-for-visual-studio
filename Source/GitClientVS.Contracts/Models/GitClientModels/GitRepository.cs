using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Contracts.Models.GitClientModels
{
    public class  GitRepository
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsPrivate { get; set; }
        public bool? HasIssues { get; set; }
        public bool? HasWiki { get; set; }
        public bool? IsForked { get; set; }
        public string Owner { get; set; }
        public string CloneUrl { get; set; }

        public GitRepository()
        {
            
        }

        public GitRepository(string name, string description, string owner, bool isPrivate)
        {
            Name = name;
            Description = description;
            Owner = owner;
            IsPrivate = isPrivate;
        }

        public GitRepository(string name, string owner, string cloneUrl)
        {
            Name = name;
            Owner = owner;
            CloneUrl = cloneUrl;
        }
    }
}
