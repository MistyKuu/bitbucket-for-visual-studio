using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;

namespace GitClientVS.UI.Views
{
    /// <summary>
    /// Interaction logic for CreatePullRequestsView.xaml
    /// </summary>
    [Export(typeof(ICreatePullRequestsView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CreatePullRequestsView : UserControl, ICreatePullRequestsView
    {
        private readonly ICreatePullRequestsViewModel _vm;

        [ImportingConstructor]
        public CreatePullRequestsView(ICreatePullRequestsViewModel vm)
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
