using System.Collections.Generic;

namespace ParseDiff
{
    public class LineDiff
    {
        public LineDiff()
        {
            
        }

        public LineDiff(LineChangeType type, int index, string content)
        {
            Type = type;
            Index = index;
            Content = content;
        }

        public LineDiff(int oldIndex, int newIndex, string content)
        {
            OldIndex = oldIndex;
            NewIndex = newIndex;
            Type = LineChangeType.Normal;
            Content = content;
        }

        public bool Add => Type == LineChangeType.Add;

        public bool Delete => Type == LineChangeType.Delete;

        public bool Normal => Type == LineChangeType.Normal;

        public string Content { get; set; }

        public int Index { get; set; }

        public int? OldIndex { get; set; }

        public int? NewIndex { get; set; }

        public LineChangeType Type { get; set; }
    }
}
