using System.Collections.Generic;
using System.Linq;
using DiffPlex;
using DiffPlex.DiffBuilder;

namespace ParseDiff
{
    public class DiffFileParser
    {
        private static SideBySideDiffBuilder _diffBuilder;
// from property holds the name of the file -> no matter what

        static DiffFileParser()
        {
            _diffBuilder = new SideBySideDiffBuilder(new Differ());
        }

        public static IEnumerable<FileDiff> Parse(string diff)
        {
            var files = Diff.Parse(diff).ToList();

            foreach (var fileDiff in files)
            {
                if (fileDiff.Type == FileChangeType.Modified)
                {

                    foreach (var chunk in fileDiff.Chunks)
                    {
                        foreach (var change in chunk.Changes)
                        {
                            if (change.Type == LineChangeType.Add)
                                change.NewIndex = change.Index;
                            else if (change.Type == LineChangeType.Delete)
                                change.OldIndex = change.Index;
                        }

                        var pairs = from added in chunk.Changes.Where(x=>x.Type == LineChangeType.Add)
                                    join deleted in chunk.Changes.Where(x => x.Type == LineChangeType.Delete) 
                                    on added.NewIndex equals deleted.OldIndex
                                    select new { Added = added, Deleted = deleted };

                        foreach (var pair in pairs)
                        {
                            var result = _diffBuilder.BuildDiffModel(pair.Deleted.Content, pair.Added.Content);
                            pair.Deleted.ChangesInLine = result.OldText;//todo REWRITE EVERYTHING TO USE THIS LIB
                            pair.Added.ChangesInLine = result.NewText;
                        }
                    }
                }
                else if (fileDiff.Type == FileChangeType.Add)
                {
                    foreach (var change in fileDiff.Chunks.Select(chunk => chunk.Changes).SelectMany(changes => changes))
                        change.NewIndex = change.Index;
                }
                else if (fileDiff.Type == FileChangeType.Delete)
                {
                    foreach (var change in fileDiff.Chunks.Select(chunk => chunk.Changes).SelectMany(changes => changes))
                        change.OldIndex = change.Index;
                }

                yield return fileDiff;
            }
        }
    }
}
