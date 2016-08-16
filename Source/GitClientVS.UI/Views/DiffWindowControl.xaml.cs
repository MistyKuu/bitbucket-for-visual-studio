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
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.UI.Helpers;
using Microsoft.VisualStudio.PlatformUI;

namespace GitClientVS.UI.Views
{
    [Export(typeof(IDiffWindowControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DiffWindowControl : UserControl, IDiffWindowControl
    {
        [ImportingConstructor]
        public DiffWindowControl()
        {
            InitializeComponent();
            Closed += delegate { OnClosed(); };
        }

        public event EventHandler Closed;

        protected virtual void OnClosed()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}