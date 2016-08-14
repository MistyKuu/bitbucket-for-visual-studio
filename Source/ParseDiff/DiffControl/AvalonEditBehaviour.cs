using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;

namespace ParseDiff.DiffControl
{
    public sealed class AvalonEditBehaviour : DependencyObject
    {
        public static bool GetIsDiffEditor(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDiffEditorProperty);
        }
        public static void SetIsDiffEditor(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDiffEditorProperty, value);
        }
        // Using a DependencyProperty as the backing store for TextBindingChanged. This enables animation, styling, binding, etc...  
        public static readonly DependencyProperty IsDiffEditorProperty =
        DependencyProperty.RegisterAttached("IsDiffEditor", typeof(bool), typeof(AvalonEditBehaviour), new PropertyMetadata(false, RendererChanged));

        private static void RendererChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextEditor textEditor = d as TextEditor;
            textEditor.TextArea.TextView.BackgroundRenderers.Add(new DiffLineBackgroundRenderer());
            textEditor.TextArea.TextView.LineTransformers.Add(new DiffLineColorizer((ChunkDiff)textEditor.DataContext));
            textEditor.TextArea.LeftMargins.Add(new TwoColumnMargin());
            textEditor.TextArea.LeftMargins.Add(DottedLineMargin.Create());
        }


        public static string GetTextBinding(DependencyObject obj)
        {
            return (string)obj.GetValue(TextBindingProperty);
        }
        public static void SetTextBinding(DependencyObject obj, string value)
        {
            obj.SetValue(TextBindingProperty, value);
        }
        // Using a DependencyProperty as the backing store for TextBindingChanged. This enables animation, styling, binding, etc...  
        public static readonly DependencyProperty TextBindingProperty =
        DependencyProperty.RegisterAttached("TextBinding", typeof(string), typeof(AvalonEditBehaviour), new PropertyMetadata(null, TextBindingChanged));
        private static void TextBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextEditor textEditor = d as TextEditor;
            if (textEditor?.Document != null)
            {
                var caretOffset = textEditor.CaretOffset;
                textEditor.Document.Text = e.NewValue.ToString();
                textEditor.CaretOffset = caretOffset;
            }
        }
    }
}
