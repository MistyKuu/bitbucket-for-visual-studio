using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces.Services;

namespace GitClientVS.Services
{
    [Export(typeof(IFileService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FileService : IFileService
    {
        public void Save(string path, string content)
        {
            var file = new FileInfo(path);
            file.Directory?.Create();
            File.WriteAllText(file.FullName, content);
        }

        public string Read(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
