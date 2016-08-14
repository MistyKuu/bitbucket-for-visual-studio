using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces.Services;
using ParseDiff;

namespace GitClientVS.Services
{
    [Export(typeof(IDiffFileParser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DiffFileParser : IDiffFileParser
    {
        public IEnumerable<FileDiff> Parse(string diff)
        {
            var files = Diff.Parse(diff).ToList();

            foreach (var fileDiff in files)
            {
                if (fileDiff.Type == FileChangeType.Modified)
                {
                    var name = fileDiff.From; // from == to?
                    var additions = fileDiff.Additions;
                    var deletions = fileDiff.Deletions;
                    
                    foreach (var change in fileDiff.Chunks.Select(chunk => chunk.Changes).SelectMany(changes => changes))
                    {
                        if (change.Type == LineChangeType.Add)
                            change.NewIndex = change.Index;
                        else if (change.Type == LineChangeType.Delete)
                            change.OldIndex = change.Index;
                    }
                }
                else if (fileDiff.Type == FileChangeType.Add)
                {

                }
                else if (fileDiff.Type == FileChangeType.Delete)
                {

                }

                yield return fileDiff;
            }
        }
    }
}
