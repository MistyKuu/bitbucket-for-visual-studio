using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GitClientVS.UI.Converters;

namespace GitClientVS.UI.Controls
{
    /// <summary>
    /// Interaction logic for ProgressBar.xaml
    /// </summary>
    public partial class ProgressBar : UserControl
    {
        public double? OverrideWidth
        {
            get { return (double?)GetValue(OverrideWidthProperty); }
            set { SetValue(OverrideWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OverrideWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OverrideWidthProperty =
            DependencyProperty.Register("OverrideWidth", typeof(double?), typeof(ProgressBar), new PropertyMetadata(null));

        public double? OverrideHeight
        {
            get { return (double?)GetValue(OverrideHeightProperty); }
            set { SetValue(OverrideHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OverrideHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OverrideHeightProperty =
            DependencyProperty.Register("OverrideHeight", typeof(double?), typeof(ProgressBar), new PropertyMetadata(null));




        public UIElement ProgressContent
        {
            get { return (UIElement)GetValue(ProgressContentProperty); }
            set { SetValue(ProgressContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProgressContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressContentProperty =
            DependencyProperty.Register("ProgressContent", typeof(UIElement), typeof(ProgressBar), new PropertyMetadata(null, OnProgressContentChanged));

        private static void OnProgressContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as ProgressBar;
            obj?.RefreshVisibility();
        }

        public ProgressBar()
        {
            InitializeComponent();
            this.IsVisibleChanged += ProgressBar_IsVisibleChanged;
            this.Loaded += ProgressBar_Loaded;
        }

        private void ProgressBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (OverrideHeight != null)
                ProgressRing.Height = OverrideHeight.Value;
            if (OverrideWidth != null)
                ProgressRing.Width = OverrideWidth.Value;
        }

        private void ProgressBar_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RefreshVisibility();
        }

        private void RefreshVisibility()
        {
            if (ProgressContent != null)
            {
                ProgressContent.Visibility = this.Visibility == Visibility.Visible
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }
    }
}
