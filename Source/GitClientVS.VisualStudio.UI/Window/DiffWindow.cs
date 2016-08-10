//------------------------------------------------------------------------------
// <copyright file="DiffWindow.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.ComponentModel.Composition;
using System.Reactive.Linq;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Infrastructure.Extensions;
using GitClientVS.Infrastructure.ViewModels;
using GitClientVS.UI.Views;
using Microsoft.VisualStudio.ComponentModelHost;
using ReactiveUI;

namespace GitClientVS.VisualStudio.UI.Window
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("0027eeb1-cbc1-4e21-a806-e3d120f7a770")]
    public class DiffWindow : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiffWindow"/> class.
        /// </summary>

        public DiffWindow() : base(null)
        {
            Caption = "Diff";

            var vm = new DiffWindowControlViewModel();
            vm.WhenAnyValue(x => x.FileDiff).Where(x => x != null).Subscribe(x => Caption = $"Diff ({x.From})");

            Content = new DiffWindowControl(vm);
        }
    }
}
