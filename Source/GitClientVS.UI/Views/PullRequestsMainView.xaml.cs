using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;

namespace GitClientVS.UI.Views
{
    /// <summary>
    /// Interaction logic for PullRequestsMainView.xaml
    /// </summary>
    [Export(typeof(IPullRequestsMainView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PullRequestsMainView : UserControl, IPullRequestsMainView
    {
        private readonly IPullRequestsMainViewModel _vm;

        [ImportingConstructor]
        public PullRequestsMainView(IPullRequestsMainViewModel vm)
        {
            _vm = vm;
            DataContext = vm;
            InitializeComponent();
            Loaded += PullRequestsMainViewView_Loaded;
        }

        private void PullRequestsMainViewView_Loaded(object sender, EventArgs e)
        {
            _vm.InitializeCommand.Execute(null);
        }
    }
}
