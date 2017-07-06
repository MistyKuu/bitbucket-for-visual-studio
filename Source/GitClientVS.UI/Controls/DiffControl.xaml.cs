using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GitClientVS.Contracts.Models;
using ICSharpCode.AvalonEdit;
using ParseDiff;

namespace GitClientVS.UI.Controls
{
    /// <summary>
    /// Interaction logic for DiffControl.xaml
    /// </summary>
    public partial class DiffControl : UserControl
    {
        public FileDiff FileDiff
        {
            get { return (FileDiff)GetValue(FileDiffProperty); }
            set { SetValue(FileDiffProperty, value); }
        }

       
        // Using a DependencyProperty as the backing store for FileDiff.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileDiffProperty =
            DependencyProperty.Register("FileDiff", typeof(FileDiff), typeof(DiffControl), new PropertyMetadata(null));

        public ObservableCollection<object> DisplayedModels
        {
            get { return (ObservableCollection<object>)GetValue(DisplayedModelsProperty); }
            set { SetValue(DisplayedModelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayedModels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayedModelsProperty =
            DependencyProperty.Register("DisplayedModels", typeof(ObservableCollection<object>), typeof(DiffControl), new PropertyMetadata(null));




        public Theme Theme
        {
            get { return (Theme)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Theme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(Theme), typeof(DiffControl), new PropertyMetadata(Theme.Light));



        public DiffControl()
        {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }


        private void UIElement_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

      
    }
}
