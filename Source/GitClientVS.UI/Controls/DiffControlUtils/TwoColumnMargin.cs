using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using ParseDiff;

namespace GitClientVS.UI.Controls.DiffControlUtils
{
    public abstract class UserControlMargin : UserControl, ITextViewConnect
    {
        /// <summary>
        /// TextView property.
        /// </summary>
        public static readonly DependencyProperty TextViewProperty =
            DependencyProperty.Register("TextView", typeof(TextView), typeof(UserControlMargin),
                new FrameworkPropertyMetadata(OnTextViewChanged));

        /// <summary>
        /// Gets/sets the text view for which line numbers are displayed.
        /// </summary>
        /// <remarks>Adding a margin to <see cref="TextArea.LeftMargins"/> will automatically set this property to the text area's TextView.</remarks>
        public TextView TextView
        {
            get { return (TextView)GetValue(TextViewProperty); }
            set { SetValue(TextViewProperty, value); }
        }

        static void OnTextViewChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            UserControlMargin margin = (UserControlMargin)dp;
            margin.wasAutoAddedToTextView = false;
            margin.OnTextViewChanged((TextView)e.OldValue, (TextView)e.NewValue);
        }

        // automatically set/unset TextView property using ITextViewConnect
        bool wasAutoAddedToTextView;

        void ITextViewConnect.AddToTextView(TextView textView)
        {
            if (this.TextView == null)
            {
                this.TextView = textView;
                wasAutoAddedToTextView = true;
            }
            else if (this.TextView != textView)
            {
                throw new InvalidOperationException("This margin belongs to a different TextView.");
            }
        }

        void ITextViewConnect.RemoveFromTextView(TextView textView)
        {
            if (wasAutoAddedToTextView && this.TextView == textView)
            {
                this.TextView = null;
                Debug.Assert(!wasAutoAddedToTextView); // setting this.TextView should have unset this flag
            }
        }

        TextDocument document;

        /// <summary>
        /// Gets the document associated with the margin.
        /// </summary>
        public TextDocument Document
        {
            get { return document; }
        }

        /// <summary>
        /// Called when the <see cref="TextView"/> is changing.
        /// </summary>
        protected virtual void OnTextViewChanged(TextView oldTextView, TextView newTextView)
        {
            if (oldTextView != null)
            {
                oldTextView.DocumentChanged -= TextViewDocumentChanged;
            }
            if (newTextView != null)
            {
                newTextView.DocumentChanged += TextViewDocumentChanged;
            }
            TextViewDocumentChanged(null, null);
        }

        void TextViewDocumentChanged(object sender, EventArgs e)
        {
            OnDocumentChanged(document, TextView != null ? TextView.Document : null);
        }

        /// <summary>
        /// Called when the <see cref="Document"/> is changing.
        /// </summary>
        protected virtual void OnDocumentChanged(TextDocument oldDocument, TextDocument newDocument)
        {
            document = newDocument;
        }
    }

    //public class TwoColumnMargin : ItemsControlMargin
    //{
    //    private double _emSize;
    //    private Typeface _typeface;
    //    private Brush _brush;
    //    private int _maxFrom = 2;
    //    private int _maxTo = 2;
    //    private FormattedText _maxFromText;
    //    private const int _margin = 10;

    //    protected override Size MeasureOverride(Size availableSize)
    //    {
    //        _typeface = Fonts.SystemTypefaces.First();
    //        _emSize = (double)GetValue(TextBlock.FontSizeProperty);
    //        _brush = new SolidColorBrush(Colors.Gray);

    //        var chunk = DataContext as ChunkDiff;

    //        SetMaxes(chunk);

    //        FormattedText text = CreateText(new string('c', _maxFrom + _maxTo));

    //        return new Size(text.Width + _margin, 0);
    //    }

    //    private void SetMaxes(ChunkDiff chunk)
    //    {
    //        var oldIndexes = chunk.Changes.Where(x => x.OldIndex != null).ToList();
    //        var newIndexes = chunk.Changes.Where(x => x.NewIndex != null).ToList();

    //        if (oldIndexes.Any())
    //            _maxFrom = oldIndexes.Select(x => " " + x.OldIndex.ToString()).Max(x => x.Length);
    //        if (newIndexes.Any())
    //            _maxTo = newIndexes.Select(x => x.NewIndex.ToString() + " ").Max(x => x.Length);

    //        _maxFromText = CreateText(new string('c', _maxFrom));
    //    }


    //    protected override void OnRender(DrawingContext drawingContext)
    //    {
    //        TextView textView = this.TextView;
    //        var chunk = textView.DataContext as ChunkDiff;

    //        if (textView != null && textView.VisualLinesValid)
    //        {
    //            this.Items.Clear();
    //            foreach (VisualLine line in textView.VisualLines)
    //            {
    //                var linenum = line.FirstDocumentLine.LineNumber - 1;
    //                if (linenum >= chunk.Changes.Count) continue;

    //                var diffLine = chunk.Changes[linenum];

    //                // FormattedText from = CreateText(diffLine.OldIndex?.ToString() ?? string.Empty);
    //                //  FormattedText to = CreateText(diffLine.NewIndex?.ToString() ?? string.Empty);

    //                var sp = new StackPanel { Orientation = Orientation.Horizontal };

    //                sp.Children.Add(new TextBlock() { Text = CreateText(diffLine.OldIndex?.ToString() ?? string.Empty).Text });
    //                sp.Children.Add(new TextBlock() { Text = CreateText(diffLine.NewIndex?.ToString() ?? string.Empty).Text });
    //                this.Items.Add(sp);
    //                //drawingContext.DrawText(from, new Point(_margin, line.VisualTop - textView.VerticalOffset));
    //                //drawingContext.DrawText(to, new Point(_maxFromText.Width + _margin, line.VisualTop - textView.VerticalOffset));
    //            }
    //        }
    //    }

    //    private FormattedText CreateText(string text)
    //    {
    //        var ft = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, _typeface, _emSize, _brush);
    //        ft.SetFontWeight(FontWeights.Light);
    //        return ft;
    //    }
    //}
}
