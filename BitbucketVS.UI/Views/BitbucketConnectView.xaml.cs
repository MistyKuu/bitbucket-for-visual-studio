using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using BitBucketVs.Contracts.Interfaces.Views;

namespace BitbucketVS.UI.Views
{
    /// <summary>
    /// Interaction logic for BitbucketConnectView.xaml
    /// </summary>
    [Export(typeof(IBitbucketConnectView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class BitbucketConnectView : UserControl, IBitbucketConnectView
    {
        public BitbucketConnectView()
        {
            InitializeComponent();
            DataContextChanged += BitbucketConnectView_DataContextChanged;
        }

        private void BitbucketConnectView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        
        }
    }
}
