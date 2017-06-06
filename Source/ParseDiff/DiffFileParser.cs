using System.Collections.Generic;
using System.Linq;
using DiffPlex;
using DiffPlex.DiffBuilder;

namespace ParseDiff
{
    public class DiffFileParser
    {
        // from property holds the name of the file -> no matter what
        public static IEnumerable<FileDiff> Parse(string diff)
        {
            var files = Diff.Parse(diff).ToList();

            foreach (var fileDiff in files)
            {
                if (fileDiff.Type == FileChangeType.Modified)
                {

                    foreach (var chunk in fileDiff.Chunks)
                    {
                        int originalDifference = chunk.OldStart - chunk.NewStart;
                        int added = 0;
                        int deleted = 0;


                        foreach (var change in chunk.Changes)
                        {
                            if (change.Type == LineChangeType.Add)
                            {
                                change.NewIndex = change.Index;
                                change.Index = change.NewIndex.Value - chunk.NewStart + added;
                                added++;
                            }
                            else if (change.Type == LineChangeType.Delete)
                            {
                                change.OldIndex = change.Index;
                                change.Index = change.OldIndex.Value - chunk.OldStart - deleted;
                                deleted++;
                            }
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
