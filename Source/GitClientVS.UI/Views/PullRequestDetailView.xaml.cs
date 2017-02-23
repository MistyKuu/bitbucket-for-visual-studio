using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;

namespace GitClientVS.UI.Views
{
    [Export(typeof(IPullRequestDetailView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PullRequestDetailView : UserControl, IPullRequestDetailView
    {

        [ImportingConstructor]
        public PullRequestDetailView(IPullRequestsDetailViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            var heightDescriptor = DependencyPropertyDescriptor.FromProperty(RowDefinition.HeightProperty, typeof(RowDefinition));
            heightDescriptor.AddValueChanged(FirstRow, HeightChanged);
        }

        private void HeightChanged(object sender, EventArgs e)
        {
            FirstRow.MaxHeight = Double.PositiveInfinity;
        }

        private void GoToCommit(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }


}
