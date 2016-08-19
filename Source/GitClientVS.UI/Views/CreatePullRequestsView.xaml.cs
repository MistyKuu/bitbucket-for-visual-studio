using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        [ImportingConstructor]
        public CreatePullRequestsView(ICreatePullRequestsViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
