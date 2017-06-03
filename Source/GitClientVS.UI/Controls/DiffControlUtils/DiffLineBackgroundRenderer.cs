using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DiffPlex.DiffBuilder.Model;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ParseDiff;

namespace GitClientVS.UI.Controls.DiffControlUtils
{
    public class DiffLineBackgroundRenderer : IBackgroundRenderer
    {
        static Pen pen;

        private readonly SolidColorBrush _removedLineBackground;
        private readonly SolidColorBrush _addedLineBackground;
        private readonly SolidColorBrush _addedWordBackground;
        private SolidColorBrush _removedWordBackground;

        public DiffLineBackgroundRenderer(
            SolidColorBrush addedLineBackground,
            SolidColorBrush removedLineBackground,
            SolidColorBrush addedWordBackground,
            SolidColorBrush removedWordBackground
            )
        {
            _addedLineBackground = addedLineBackground;
            _addedWordBackground = addedWordBackground;

            _removedLineBackground = removedLineBackground;
            _removedWordBackground = removedWordBackground;

            var blackBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)); blackBrush.Freeze();
            pen = new Pen(blackBrush, 0.0);
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Background; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {

            var chunk = (ChunkDiff)textView.DataContext;

            foreach (VisualLine v in textView.VisualLines)
            {
                var rc = BackgroundGeometryBuilder.GetRectsFromVisualSegment(textView, v, 0, 1000).First();
                // NB: This lookup to fetch the doc line number isn't great, we could
                // probably do it once then just increment.
                var linenum = v.FirstDocumentLine.LineNumber - 1;
                if (linenum >= chunk.Changes.Count) continue;

                var diffLine = chunk.Changes[linenum];

                if (diffLine.Type == LineChangeType.Normal) continue;

                var brush = default(Brush);
                switch (diffLine.Type)
                {
                    case LineChangeType.Add:
                        brush = _addedLineBackground;
                        break;
                    case LineChangeType.Delete:
                        brush = _removedLineBackground;
                        break;
                }

                drawingContext.DrawRectangle(brush, pen, new Rect(0, rc.Top, textView.ActualWidth, rc.Height));
            }
        }
    }

    public class OffsetColorizer : DocumentColorizingTransformer
    {
        private readonly ChunkDiff _chunkDiff;

        public OffsetColorizer(ChunkDiff chunkDiff)
        {
            _chunkDiff = chunkDiff;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            if (line.Length == 0)
                return;


            //if (line.Offset < StartOffset || line.Offset > EndOffset)
            //    return;

            //int start = line.Offset > StartOffset ? line.Offset : StartOffset;
            //int end = EndOffset > line.EndOffset ? line.EndOffset : EndOffset;


            var change = _chunkDiff.Changes[line.LineNumber - 1];
            foreach (var changeLine in change.ChangesInLine?.Lines
                       .Where(x => x.Type != ChangeType.Imaginary && x.Type != ChangeType.Unchanged) ?? new List<DiffPiece>())
            {
                foreach (var diffPiece in changeLine.SubPieces
                    .Where(x => x.Type != ChangeType.Imaginary && x.Type != ChangeType.Unchanged))
                {

                    var start = line.Offset + change.Content.IndexOf(diffPiece.Text, StringComparison.InvariantCultureIgnoreCase);
                    var end = start + diffPiece.Text.Length;

                    ChangeLinePart(start, end, element => element.TextRunProperties.SetBackgroundBrush(
                        diffPiece.Type == ChangeType.Deleted ? Brushes.OrangeRed : Brushes.LightGreen));
                }
            }



        }
    }
}
