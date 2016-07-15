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
using BitBucketVs.Contracts.Interfaces.ViewModels;
using BitBucketVs.Contracts.Interfaces.Views;

namespace BitbucketVS.UI.Views
{
    /// <summary>
    /// Interaction logic for ConnectSectionView.xaml
    /// </summary>
    [Export(typeof(IConnectSectionView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ConnectSectionView : UserControl, IConnectSectionView
    {
        [ImportingConstructor]
        public ConnectSectionView(IConnectSectionViewModel connectSectionViewModel)
        {
            InitializeComponent();
            DataContext = connectSectionViewModel;
        }
    }
}
