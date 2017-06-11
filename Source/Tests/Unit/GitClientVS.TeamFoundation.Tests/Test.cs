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
    [Ignore("Internal purpose")]
    [TestFixture]
    class Test12
    {
        [Test]
        public void Test1()
        {
           // var cu = new SideBySideDiffBuilder(new Differ());
            // var cc = cu.BuildDiffModel("public class Startup", "publicStartup 123");
            //var cc2 = cu.BuildDiffModel("options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();", "options.SeriakkkoolizerSettings.ContractResolver = new CamelCasePrkokjopertyNamesContractResolver();");
            //var cc3 = cu.BuildDiffModel("public void ConfigureServices(IServiceCollection services)", "public void ConfigureServices(ISollection services)");
            //var cc4 = cu.BuildDiffModel(@"-""Microsoft.AspNetCore.ReactServices"": ""1.0.0-*""", @"+""Microsoft.AspNetCore.RqwdqwdwqdqwdwqdqwdqeactServices"": ""1.0.0-*""");



           // var diffBuilder = new Differ();
           // var diff = diffBuilder.CreateCharacterDiffs("public class Startup", "publicStartup 123", false);

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
