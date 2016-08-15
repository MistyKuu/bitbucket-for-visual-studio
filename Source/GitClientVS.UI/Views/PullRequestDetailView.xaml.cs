using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;

namespace GitClientVS.UI.Views
{
    [Export(typeof(IPullRequestDetailView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PullRequestDetailView : UserControl, IPullRequestDetailView
    {
        private readonly IPullRequestsDetailViewModel _detailsViewModel;

        [ImportingConstructor]
        public PullRequestDetailView(IPullRequestsDetailViewModel detailsViewModel)
        {
            InitializeComponent();
            _detailsViewModel = detailsViewModel;
            DataContext = _detailsViewModel;
        }


        public ICommand InitializeCommand => _detailsViewModel.InitializeCommand;

    }


}
