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
    /// Interaction logic for ProxyLoginDialogView.xaml
    /// </summary>
    [Export(typeof(IProxyLoginDialogView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ProxyLoginDialogView : MetroWindow, IProxyLoginDialogView
    {
        private ControlTemplate _actualPbTemplate;

        [ImportingConstructor]
        public ProxyLoginDialogView(IProxyLoginDialogViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            Owner = Application.Current.MainWindow;
            vm.Closed += (s, ev) =>
            {
                DialogResult = ev;
                Close();
            };
            Loaded += LoadedWindow;
            vm.Initialize();
        }

        private void LoadedWindow(object sender, RoutedEventArgs e)
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
