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

namespace GitClientVS.UI.Views
{
    /// <summary>
    /// Interaction logic for LoginDialogView.xaml
    /// </summary>
    [Export(typeof(ILoginDialogView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class LoginDialogView : MetroWindow, ILoginDialogView
    {
        private ControlTemplate _actualPbTemplate;

        [ImportingConstructor]
        public LoginDialogView(ILoginDialogViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Owner = Application.Current.MainWindow;
            vm.Closed += delegate { Close(); };
            Loaded += LoginDialogView_Loaded;
        }

        private void LoginDialogView_Loaded(object sender, RoutedEventArgs e)
        {
            _actualPbTemplate = Validation.GetErrorTemplate(PasswordBox);
            Validation.SetErrorTemplate(PasswordBox, new ControlTemplate());
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            Validation.SetErrorTemplate(PasswordBox, _actualPbTemplate);
        }
    }
}
