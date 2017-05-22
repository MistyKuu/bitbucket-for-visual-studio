using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using NUnit.Framework;

namespace GitClientVS.Services.Tests
{
    [TestFixture]
    class Test12
    {
        [Test]
        public void Test1()
        {
            using (Repository repo = new Repository("C:\\Users\\misty\\Source\\Repos2\\NetCoreTest"))
            {
                foreach (TreeEntryChanges c in repo.Diff.Compare<TreeChanges>())
                {
                    Console.WriteLine(c);
                }

                //    var fileHistory = repo.Commits.QueryBy("project.json").ToList();
                //    foreach (var version in fileHistory)
                //    {
                //        var commit = version.Commit;
                //        var path = version.Path;
                //        Console.WriteLine(path);
                //        foreach (var tree in commit.Tree)
                //        {
                //            Console.WriteLine("name: " + tree.Name);
                //            var blob = tree.Target as Blob;
                //            if (blob != null)
                //                Console.WriteLine(blob.GetContentText());
                //        }
                //    }
                //}
            }
        }
    }
}
