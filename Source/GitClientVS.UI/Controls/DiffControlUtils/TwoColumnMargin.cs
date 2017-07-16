using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ParseDiff;

namespace GitClientVS.UI.Controls.DiffControlUtils
{
    public class TestMargin : AbstractMargin
    {
    }

    public class TwoColumnMargin : LineNumberMargin
    {
        private double _emSize;
        private Typeface _typeface;
        private Brush _brush;
        private int _maxFrom = 2;
        private int _maxTo = 2;
        private FormattedText _maxFromText;
        private const int _margin = 10;

        protected override Size MeasureOverride(Size availableSize)
        {
            _typeface = Fonts.SystemTypefaces.First();
            _emSize = (double)GetValue(TextBlock.FontSizeProperty);
            _brush = new SolidColorBrush(Colors.Gray);

            var chunk = DataContext as ChunkDiff;

            SetMaxes(chunk);

            FormattedText text = CreateText(new string('c', _maxFrom + _maxTo));

            return new Size(text.Width + _margin, 0);
        }

        private void SetMaxes(ChunkDiff chunk)
        {
            var oldIndexes = chunk.Changes.Where(x => x.OldIndex != null).ToList();
            var newIndexes = chunk.Changes.Where(x => x.NewIndex != null).ToList();

            if (oldIndexes.Any())
                _maxFrom = oldIndexes.Select(x => " " + x.OldIndex.ToString()).Max(x => x.Length);
            if (newIndexes.Any())
                _maxTo = newIndexes.Select(x => x.NewIndex.ToString() + " ").Max(x => x.Length);

            _maxFromText = CreateText(new string('c', _maxFrom));
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            TextView textView = this.TextView;
            var chunk = textView.DataContext as ChunkDiff;

            if (textView != null && textView.VisualLinesValid)
            {

                foreach (VisualLine line in textView.VisualLines)
                {
                    var linenum = line.FirstDocumentLine.LineNumber - 1;
                    if (linenum >= chunk.Changes.Count) continue;

                    var diffLine = chunk.Changes[linenum];

                    FormattedText from = CreateText(diffLine.OldIndex?.ToString() ?? string.Empty);
                    FormattedText to = CreateText(diffLine.NewIndex?.ToString() ?? string.Empty);

                    drawingContext.DrawText(from, new Point(_margin, line.VisualTop - textView.VerticalOffset));
                    drawingContext.DrawText(to, new Point(_maxFromText.Width + _margin, line.VisualTop - textView.VerticalOffset));
                }
            }
        }

        private FormattedText CreateText(string text)
        {
            var ft = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, _typeface, _emSize, _brush);
            ft.SetFontWeight(FontWeights.Light);
            return ft;
        }
    }
}
