using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using DiffPlex.DiffBuilder.Model;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ParseDiff;

namespace GitClientVS.UI.Controls.DiffControlUtils
{
    public class OffsetColorizer : DocumentColorizingTransformer
    {

        private readonly ChunkDiff _chunkDiff;
        private readonly SolidColorBrush _addedWordBackground;
        private readonly SolidColorBrush _removedWorkBackground;

        public OffsetColorizer(ChunkDiff chunkDiff, SolidColorBrush addedWordBackground, SolidColorBrush removedWorkBackground)
        {
            _chunkDiff = chunkDiff;
            _addedWordBackground = addedWordBackground;
            _removedWorkBackground = removedWorkBackground;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            if (line.Length == 0)
                return;

            var change = _chunkDiff.Changes[line.LineNumber - 1];
            foreach (var changeLine in change.ChangesInLine?.Lines
                                           .Where(x => x.Type != ChangeType.Imaginary && x.Type != ChangeType.Unchanged) ?? new List<DiffPiece>())
            {
                foreach (var diffPiece in changeLine
                    .SubPieces
                    .Where(x => x.Type != ChangeType.Imaginary && x.Type != ChangeType.Unchanged))
                {
                    var start = line.Offset + change.Content.IndexOf(diffPiece.Text, StringComparison.InvariantCulture);
                    var end = start + diffPiece.Text.Length;

                    ChangeLinePart(start, end, element => element.TextRunProperties.SetBackgroundBrush(
                        diffPiece.Type == ChangeType.Deleted ? _removedWorkBackground : _addedWordBackground));
                }
            }
        }
    }
}