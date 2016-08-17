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
            Loaded += PullRequestDetailView_Loaded;
        }

        private void PullRequestDetailView_Loaded(object sender, RoutedEventArgs e)
        {//lovely code behind
            //MainSectionGrid.Measure(new Size(PqDetailView.ActualWidth, Double.PositiveInfinity));
            //ExpandButton.Visibility = MainSectionGrid.DesiredSize.Height >= MainSectionGrid.MaxHeight
            //    ? Visibility.Visible
            //    : Visibility.Collapsed;
        }

        public ICommand InitializeCommand => _detailsViewModel.InitializeCommand;
    }


}
