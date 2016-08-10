//------------------------------------------------------------------------------
// <copyright file="DiffWindowControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;

namespace GitClientVS.UI.Views
{
    [Export(typeof(IDiffWindowControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DiffWindowControl : UserControl, IDiffWindowControl
    {
        [ImportingConstructor]
        public DiffWindowControl(IDiffWindowControlViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}