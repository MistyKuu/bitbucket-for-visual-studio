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

namespace BitbucketVS.VisualStudio.UI.Pages.TestPage
{
    /// <summary>
    /// Interaction logic for TestPage.xaml
    /// </summary>
    public partial class TestPageView : UserControl
    {

        public static readonly DependencyProperty ParentSectionProperty = DependencyProperty.Register("ParentSection", typeof(TestPage), typeof(TestPageView));

        public TestPageView()
        {
            InitializeComponent();
        }

        public TestPage ParentSection
        {
            get { return (TestPage)GetValue(ParentSectionProperty); }

            set { SetValue(ParentSectionProperty, value); }
        }
    }
}
