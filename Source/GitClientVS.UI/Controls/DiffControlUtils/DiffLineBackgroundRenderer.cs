using System.Linq;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using ParseDiff;

namespace GitClientVS.UI.Controls.DiffControlUtils
{
    public class DiffLineBackgroundRenderer : IBackgroundRenderer
    {
        static Pen pen;

        private readonly SolidColorBrush _removedLineBackground;
        private readonly SolidColorBrush _addedLineBackground;

        public DiffLineBackgroundRenderer(
            SolidColorBrush addedLineBackground,
            SolidColorBrush removedLineBackground
            )
        {
            _addedLineBackground = addedLineBackground;
            _removedLineBackground = removedLineBackground;

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
}
