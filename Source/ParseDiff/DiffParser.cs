namespace ParseDiff
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class DiffParser
    {
        const string noeol = "\\ No newline at end of file";
        const string devnull = "/dev/null";

        private delegate void ParserAction(string line, Match m);

        private List<FileDiff> files = new List<FileDiff>();
        private int in_del, in_add;

        private ChunkDiff current = null;
        private FileDiff file = null;

        private int oldStart, newStart;
        private int oldLines, newLines;

        private readonly HandlerCollection schema;

        public DiffParser()
        {
            schema = new HandlerCollection
            {
                    { @"^diff\s", Start },
                    { @"^new file mode \d+$", NewFile },
                    { @"^deleted file mode \d+$", DeletedFile },
                    { @"^index\s[\da-zA-Z]+\.\.[\da-zA-Z]+(\s(\d+))?$", Index },
                    { @"^---\s", FromFile },
                    { @"^\+\+\+\s", ToFile },
                    { @"^@@\s+\-(\d+),?(\d+)?\s+\+(\d+),?(\d+)?\s@@", Chunk },
                    { @"^-", DeleteLine },
                    { @"^\+", AddLine }
            };
        }

        public IEnumerable<FileDiff> Run(IEnumerable<string> lines)
        {
            foreach (var line in lines)
                if (!ParseLine(line))
                    ParseNormalLine(line);

            return files;
        }

        private void Start(string line)
        {
            file = new FileDiff();
            files.Add(file);

            if (file.To == null && file.From == null)
            {
                var fileNames = ParseFileNames(line);

                if (fileNames != null)
                {
                    file.From = fileNames[0];
                    file.To = fileNames[1];
                }
            }
        }

        private void Restart()
        {
            if (file == null || file.Chunks.Count != 0)
                Start(null);
        }

        private void NewFile()
        {
            Restart();
            file.Type = FileChangeType.Add;
            file.From = devnull;
        }

        private void DeletedFile()
        {
            Restart();
            file.Type = FileChangeType.Delete;
            file.To = devnull;
        }

        private void Index(string line)
        {
            Restart();
            file.Index = line.Split(' ').Skip(1);
        }

        private void FromFile(string line)
        {
            Restart();
            file.From = ParseFileName(line);
        }

        private void ToFile(string line)
        {
            Restart();
            file.To = ParseFileName(line);
        }

        private void Chunk(string line, Match match)
        {
            in_del = oldStart = int.Parse(match.Groups[1].Value);
            oldLines = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : 0;
            in_add = newStart = int.Parse(match.Groups[3].Value);
            newLines = match.Groups[4].Success ? int.Parse(match.Groups[4].Value) : 0;
            current = new ChunkDiff(
                content: line,
                oldStart: oldStart,
                oldLines: oldLines,
                newStart: newStart,
                newLines: newLines
            );
            file.Chunks.Add(current);
        }

        private void DeleteLine(string line)
        {
            current.Changes.Add(new LineDiff(type: LineChangeType.Delete, index: in_del++, content: line));
            file.Deletions++;
        }

        private void AddLine(string line)
        {
            current.Changes.Add(new LineDiff(type: LineChangeType.Add, index: in_add++, content: line));
            file.Additions++;
        }


        private void ParseNormalLine(string line)
        {
            if (file == null) return;

            if (string.IsNullOrEmpty(line)) return;

            if (line == noeol)
                return;

            current.Changes.Add(new LineDiff(
                oldIndex: in_del++,
                newIndex: in_add++,
                content: line));
        }

        private bool ParseLine(string line)
        {
            foreach (var p in schema)
            {
                var m = p.Expression.Match(line);
                if (m.Success)
                {
                    p.Action(line, m);
                    return true;
                }
            }

            return false;
        }

        private static string[] ParseFileNames(string s)
        {
            if (string.IsNullOrEmpty(s)) return null;
            return s
                .Split(' ')
                .Reverse().Take(2).Reverse()
                .Select(fileName => Regex.Replace(fileName, @"^(a|b)\/", "")).ToArray();
        }

        private static string ParseFileName(string s)
        {
            s = s.TrimStart('-', '+');
            s = s.Trim();

            // ignore possible time stamp
            var t = new Regex(@"\t.*|\d{4}-\d\d-\d\d\s\d\d:\d\d:\d\d(.\d+)?\s(\+|-)\d\d\d\d").Match(s);
            if (t.Success)
            {
                s = s.Substring(0, t.Index).Trim();
            }

            // ignore git prefixes a/ or b/
            return Regex.IsMatch(s, @"^(a|b)\/")
                ? s.Substring(2)
                : s;
        }

        private class HandlerRow
        {
            public HandlerRow(Regex expression, Action<string, Match> action)
            {
                Expression = expression;
                Action = action;
            }

            public Regex Expression { get; }

            public Action<string, Match> Action { get; }
        }

        private class HandlerCollection : IEnumerable<HandlerRow>
        {
            private List<HandlerRow> handlers = new List<HandlerRow>();

            public void Add(string expression, Action action)
            {
                handlers.Add(new HandlerRow(new Regex(expression), (line, m) => action()));
            }

            public void Add(string expression, Action<string> action)
            {
                handlers.Add(new HandlerRow(new Regex(expression), (line, m) => action(line)));
            }

            public void Add(string expression, Action<string, Match> action)
            {
                handlers.Add(new HandlerRow(new Regex(expression), action));
            }

            public IEnumerator<HandlerRow> GetEnumerator()
            {
                return handlers.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return handlers.GetEnumerator();
            }
        }
    }
}
