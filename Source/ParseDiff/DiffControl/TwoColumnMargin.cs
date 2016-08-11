using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

namespace ParseDiff.DiffControl
{
    public class TwoColumnMargin : LineNumberMargin
    {
        private double _emSize;
        private Typeface _typeface;
        private Brush _brush;
        private int _maxFrom;
        private int _maxTo;
        private FormattedText _maxFromText;
        private const int _margin = 10;

        protected override Size MeasureOverride(Size availableSize)
        {
            _typeface = Fonts.SystemTypefaces.First();
            _emSize = (double)GetValue(TextBlock.FontSizeProperty);
            _brush = new SolidColorBrush(Colors.Gray);

            var chunk = DataContext as ChunkDiff;

            _maxFrom = chunk.Changes.Where(x => x.OldIndex != null).Select(x => " " + x.OldIndex.ToString()).Max(x => x.Length);
            _maxTo = chunk.Changes.Where(x => x.OldIndex != null).Select(x => x.NewIndex.ToString() + " ").Max(x => x.Length);
            _maxFromText = CreateText(new string('c', _maxFrom));

            FormattedText text = CreateText(new string('c', _maxFrom + _maxTo));

            return new Size(text.Width + _margin, 0);
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
