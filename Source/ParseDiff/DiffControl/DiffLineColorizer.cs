using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;

namespace ParseDiff.DiffControl
{
    public class DiffLineColorizer : DocumentColorizingTransformer
    {
        private readonly ChunkDiff _chunkDiff;

        public DiffLineColorizer(ChunkDiff chunkDiff)
        {
            _chunkDiff = chunkDiff;
        }

        protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line)
        {
            var change = _chunkDiff.Changes[line.LineNumber - 1];
            if (!line.IsDeleted && (change.Type == LineChangeType.Add || change.Type == LineChangeType.Delete))
            {
                ChangeLinePart(line.Offset, line.EndOffset, el =>
                {
                    el.TextRunProperties.SetForegroundBrush(Brushes.Black);
                });
            }
        }
    }
}
