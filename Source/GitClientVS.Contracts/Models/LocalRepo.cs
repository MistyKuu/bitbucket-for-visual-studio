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
        public string Path { get; set; }

        public LocalRepo(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
