//------------------------------------------------------------------------------
// <copyright file="DiffWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.UI.Helpers;
using Microsoft.VisualStudio.PlatformUI;

namespace GitClientVS.UI.Views
{
    [Export(typeof(IPullRequestsWindowContainer))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PullRequestsWindowContainer : UserControl, IPullRequestsWindowContainer
    {

        [ImportingConstructor]
        public PullRequestsWindowContainer()
        {
            InitializeComponent();
            DataContextChanged += PullRequestsWindowContainer_DataContextChanged;
        }

        private void PullRequestsWindowContainer_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ViewModel != null)
                ViewModel.Closed -= CloseWindow;

            ViewModel = DataContext as IPullRequestsWindowContainerViewModel;

            if (ViewModel != null)
                ViewModel.Closed += CloseWindow;
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            Window.Close();
        }

        public IPullRequestsWindowContainerViewModel ViewModel { get; set; }
        public IPullRequestsWindow Window { get; set; }
    }
}