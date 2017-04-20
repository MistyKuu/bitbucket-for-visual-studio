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
using GitClientVS.Contracts;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using MahApps.Metro.Controls;
using Microsoft.VisualStudio.PlatformUI;
using GitClientVS.Infrastructure.Extensions;

namespace GitClientVS.UI.Views
{
    /// <summary>
    /// Interaction logic for CloneRepositoriesDialogView.xaml
    /// </summary>
    [Export(typeof(ICreateRepositoriesDialogView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CreateRepositoriesDialogView : MetroWindow, ICreateRepositoriesDialogView
    {

        [ImportingConstructor]
        public CreateRepositoriesDialogView(ICreateRepositoriesDialogViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Owner = Application.Current.MainWindow;
            vm.Closed += delegate { Close(); };
            vm.Initialize();
        }
    }
}
