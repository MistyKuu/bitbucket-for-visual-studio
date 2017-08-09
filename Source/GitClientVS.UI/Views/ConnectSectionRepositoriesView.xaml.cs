using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;

namespace GitClientVS.UI.Views
{
    public partial class ConnectSectionRepositoriesView : UserControl
    {
        public ConnectSectionRepositoriesView()
        {
            InitializeComponent();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ChangeActiveRepo();
        }

        private void ListBox_MouseClickFromContext(object sender, RoutedEventArgs e)
        {
            ChangeActiveRepo();
        }

        private void ChangeActiveRepo()
        {
            GetViewModel()?.ChangeActiveRepo();
        }

        private void FileExplorer_OnClick(object sender, RoutedEventArgs e)
        {
            var localPath = GetViewModel()?.SelectedRepository?.LocalPath;
            if (localPath != null)
                Process.Start(localPath);
        }

        private IConnectSectionViewModel GetViewModel()
        {
            return (this.DataContext as IConnectSectionViewModel);
        }

        private void CommandPrompt_OnClick(object sender, RoutedEventArgs e)
        {
            var localPath = GetViewModel()?.SelectedRepository?.LocalPath;
            if (localPath != null)
            {
                var proc1 = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = localPath,
                    FileName = @"cmd.exe",
                    WindowStyle = ProcessWindowStyle.Normal
                };

                Process.Start(proc1);
            }
        }
    }
}
