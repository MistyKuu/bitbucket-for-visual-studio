using System;
using System.Linq;

namespace ParseDiff
{
    using System.Collections.Generic;

    public class ChunkDiff
    {
        public ChunkDiff(string content, int oldStart, int oldLines, int newStart, int newLines)
        {
            Content = content;
            OldStart = oldStart;
            OldLines = oldLines;
            NewStart = newStart;
            NewLines = newLines;
        }

        public ChunkDiff()
        {
            
        }

        public List<LineDiff> Changes { get; set; } = new List<LineDiff>();

        public string Content { get; }

        public int OldStart { get; }

        public int OldLines { get; }

        public int NewStart { get; }

        public int NewLines { get; }

        public string Text
        {
            get { return string.Join(Environment.NewLine, Changes.Select(x => x.Content)); }
        }
    }
}
