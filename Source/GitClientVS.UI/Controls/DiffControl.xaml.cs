using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ParseDiff;

namespace GitClientVS.UI.Controls
{
    /// <summary>
    /// Interaction logic for DiffControl.xaml
    /// </summary>
    public partial class DiffControl : UserControl
    {
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

        public FileDiff FileDiff
        {
            get { return (FileDiff)GetValue(FileDiffProperty); }
            set { SetValue(FileDiffProperty, value); }
        }

        public string HighLightStyle
        {
            get
            {
                if (FileDiff == null)
                    return HighlightMappings["cs"];

                var splitted = FileDiff.From.Split('.');
                if (splitted.Length < 2)
                    return HighlightMappings["cs"];

                var ext = splitted.Last();

                if (HighlightMappings.ContainsKey(ext))
                    return HighlightMappings[ext];

                return HighlightMappings["cs"];
            }
        }

        // Using a DependencyProperty as the backing store for FileDiff.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileDiffProperty =
            DependencyProperty.Register("FileDiff", typeof(FileDiff), typeof(DiffControl), new PropertyMetadata(null));


        public DiffControl()
        {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }
    }
}
