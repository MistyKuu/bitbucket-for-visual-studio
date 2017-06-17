using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Contracts.Models
{
    public class LocalRepo
    {
        public string Name { get; set; }
        public string LocalPath { get; set; }
        public string ClonePath { get; set; }
        public bool IsActive { get; set; }

        public LocalRepo(string name, string localPath, string clonePath)
        {
            Name = name;
            ClonePath = clonePath;
            LocalPath = localPath;
        }
    }
}
