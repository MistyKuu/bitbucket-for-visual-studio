using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GitClientVS.Contracts.Models;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ParseDiff;

namespace GitClientVS.UI.Controls.DiffControlUtils
{
    public sealed class AvalonEditBehaviour : DependencyObject
    {

        private static readonly SolidColorBrush DarkAddedBackground = new SolidColorBrush(Color.FromRgb(3, 192, 60));
        private static readonly SolidColorBrush DarkRemovedBackground = new SolidColorBrush(Color.FromRgb(194, 59, 34));

        private static readonly SolidColorBrush LightAddedBackground = new SolidColorBrush(Color.FromRgb(0xdd, 0xff, 0xdd));
        private static readonly SolidColorBrush LightRemovedBackground = new SolidColorBrush(Color.FromRgb(0xff, 0xdd, 0xdd));

        private static readonly Dictionary<string, string> HighlightMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            ["cshtml"] = "ASP / XHTML",
            ["html"] = "HTML",
            ["js"] = "JavaScript",
            ["xml"] = "XML",
            ["vb"] = "VB.NET",
            ["cs"] = "C#",
            ["cpp"] = "C++",
            ["java"] = "Java",
            ["php"] = "PHP",
            ["patch"] = "Patch",
            ["text"] = "TeX"
        };

        private static string HighLightStyle(FileDiff fileDiff)
        {
            if (fileDiff == null)
                return HighlightMappings["xml"];

            var splitted = fileDiff.From.Split('.');
            if (splitted.Length < 2)
                return HighlightMappings["xml"];

            var ext = splitted.Last();

            if (HighlightMappings.ContainsKey(ext))
                return HighlightMappings[ext];

            return HighlightMappings["xml"];
        }

        public static Theme GetTheme(DependencyObject obj)
        {
            return (Theme)obj.GetValue(ThemeProperty);
        }
        public static void SetTheme(DependencyObject obj, Theme value)
        {
            obj.SetValue(ThemeProperty, value);
        }
        // Using a DependencyProperty as the backing store for TextBindingChanged. This enables animation, styling, binding, etc...  
        public static readonly DependencyProperty ThemeProperty =
        DependencyProperty.RegisterAttached("Theme", typeof(Theme), typeof(AvalonEditBehaviour), new PropertyMetadata(Theme.Light, ThemeChanged));

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
        DependencyProperty.RegisterAttached("IsDiffEditor", typeof(bool), typeof(AvalonEditBehaviour), new PropertyMetadata(false, BehaviourAttached));

        public static FileDiff GetFileDiff(DependencyObject obj)
        {
            return (FileDiff)obj.GetValue(FileDiffProperty);
        }
        public static void SetFileDiff(DependencyObject obj, FileDiff value)
        {
            obj.SetValue(FileDiffProperty, value);
        }
        // Using a DependencyProperty as the backing store for TextBindingChanged. This enables animation, styling, binding, etc...  
        public static readonly DependencyProperty FileDiffProperty =
        DependencyProperty.RegisterAttached("FileDiff", typeof(FileDiff), typeof(AvalonEditBehaviour), new PropertyMetadata(null, FileDiffChanged));

        private static void ThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextEditor textEditor = d as TextEditor;
            ChangeBackgroundRenderer(textEditor, GetTheme(textEditor));
        }

        private static void FileDiffChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextEditor textEditor = d as TextEditor;
            textEditor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition(HighLightStyle((FileDiff)e.NewValue));
        }

        private static void BehaviourAttached(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextEditor textEditor = d as TextEditor;
            // textEditor.TextArea.TextView.LineTransformers.Add(new DiffLineColorizer((ChunkDiff)textEditor.DataContext));
            textEditor.TextArea.LeftMargins.Add(new TwoColumnMargin());
            textEditor.TextArea.LeftMargins.Add(DottedLineMargin.Create());
            ChangeBackgroundRenderer(textEditor, GetTheme(textEditor));
        }


        private static void ChangeBackgroundRenderer(TextEditor textEditor, Theme theme)
        {
            var diffBackgroundRenderer = textEditor.TextArea.TextView.BackgroundRenderers.FirstOrDefault(x => x.GetType() == typeof(DiffLineBackgroundRenderer));
            if (diffBackgroundRenderer != null)
                textEditor.TextArea.TextView.BackgroundRenderers.Remove(diffBackgroundRenderer);

            SolidColorBrush addedBg;
            SolidColorBrush removedBg;

            if (theme == Theme.Light)
            {
                addedBg = LightAddedBackground;
                removedBg = LightRemovedBackground;
            }
            else
            {
                addedBg = DarkAddedBackground;
                removedBg = DarkRemovedBackground;
            }
            textEditor.TextArea.TextView.BackgroundRenderers.Add(new DiffLineBackgroundRenderer(addedBg, removedBg));
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
