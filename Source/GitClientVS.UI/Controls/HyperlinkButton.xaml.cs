using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace GitClientVS.UI.Controls
{
    /// <summary>
    /// Interaction logic for HyperlinkButton.xaml
    /// </summary>
    public partial class HyperlinkButton : UserControl
    {

        public static readonly DependencyProperty HyperlinkContentProperty =
         DependencyProperty.Register("HyperlinkContent", typeof(string), typeof(HyperlinkButton),
             new PropertyMetadata(null));

        public static readonly DependencyProperty HyperlinkForegroundProperty =
         DependencyProperty.Register("HyperlinkForeground", typeof(Brush), typeof(HyperlinkButton),
             new PropertyMetadata(null));

        public string HyperlinkContent
        {
            get { return (string)GetValue(HyperlinkContentProperty); }
            set { SetValue(HyperlinkContentProperty, value); }
        }

        public Brush HyperlinkForeground
        {
            get { return (Brush)GetValue(HyperlinkForegroundProperty); }
            set { SetValue(HyperlinkForegroundProperty, value); }
        }

        public string GoToLink
        {
            get { return (string)GetValue(GoToLinkProperty); }
            set { SetValue(GoToLinkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GoToLink.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GoToLinkProperty =
            DependencyProperty.Register("GoToLink", typeof(string), typeof(HyperlinkButton), new PropertyMetadata(null));


        public HyperlinkButton()
        {
            InitializeComponent();
        }

        private void NavigateButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(GoToLink);
        }
    }
}
