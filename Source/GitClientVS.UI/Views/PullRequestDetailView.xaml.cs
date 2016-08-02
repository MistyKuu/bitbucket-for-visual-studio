using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;

namespace GitClientVS.UI.Views
{
    public partial class PullRequestDetailView : UserControl
    {
        public PullRequestDetailView()
        {
            InitializeComponent();
        }
    }
}
