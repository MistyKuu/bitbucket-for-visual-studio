using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.UI.Behaviours;
using GitClientVS.UI.Helpers;

namespace GitClientVS.UI.Views
{
    /// <summary>
    /// Interaction logic for PullRequestsMainView.xaml
    /// </summary>
    [Export(typeof(IPullRequestsMainView))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class PullRequestsMainView : UserControl, IPullRequestsMainView
    {
        private readonly IPullRequestsMainViewModel _vm;

        [ImportingConstructor]
        public PullRequestsMainView(IPullRequestsMainViewModel vm)
        {
            _vm = vm;
            DataContext = vm;
            InitializeComponent();
        }
    }
}
