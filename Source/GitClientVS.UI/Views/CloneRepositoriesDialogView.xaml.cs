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
using Microsoft.VisualStudio.PlatformUI;

namespace GitClientVS.UI.Views
{
    /// <summary>
    /// Interaction logic for CloneRepositoriesDialogView.xaml
    /// </summary>
    [Export(typeof(ICloneRepositoriesDialogView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CloneRepositoriesDialogView : DialogWindow, ICloneRepositoriesDialogView
    {
        private readonly ICloneRepositoriesDialogViewModel _vm;

        [ImportingConstructor]
        public CloneRepositoriesDialogView(ICloneRepositoriesDialogViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            _vm = vm;
            vm.Closed += delegate { Close(); };
            Loaded += CloneRepositoriesView_Loaded;
        }

        private async void CloneRepositoriesView_Loaded(object sender, EventArgs e)
        {
            await _vm.InitializeAsync();
        }
    }
}
