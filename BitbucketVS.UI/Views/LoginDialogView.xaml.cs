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
using BitBucketVs.Contracts;
using BitBucketVs.Contracts.Interfaces.Views;
using Microsoft.VisualStudio.PlatformUI;

namespace BitbucketVS.UI.Views
{
    /// <summary>
    /// Interaction logic for LoginDialogView.xaml
    /// </summary>
    [Export(typeof(ILoginDialogView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class LoginDialogView : DialogWindow, ILoginDialogView
    {
        public LoginDialogView()
        {
            InitializeComponent();
        }
    }
}
