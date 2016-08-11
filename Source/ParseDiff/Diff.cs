namespace ParseDiff
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Diff
    {
        public static IEnumerable<FileDiff> Parse(string input, string lineEnding = "\n")
        {
            if (string.IsNullOrWhiteSpace(input)) return Enumerable.Empty<FileDiff>();

            var lines = input.Split(new[] { lineEnding }, StringSplitOptions.None);

            if (lines.Length == 0) return Enumerable.Empty<FileDiff>();

            var parser = new DiffParser();

            return parser.Run(lines);
        }
    }
}
