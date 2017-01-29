//------------------------------------------------------------------------------
// <copyright file="GitClientVSPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Infrastructure.Extensions;
using GitClientVS.UI.Helpers;
using GitClientVS.VisualStudio.UI.Settings;
using GitClientVS.VisualStudio.UI.Window;
using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.Win32;
using IServiceProvider = System.IServiceProvider;

namespace GitClientVS.VisualStudio.UI
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    /// 
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(GuidList.guidBitbuketPkgString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(GitExtensionsId)]
    [ProvideToolWindow(typeof(DiffWindow), Style = VsDockStyle.MDI, Orientation = ToolWindowOrientation.Left, MultiInstances = true, Transient = true)]
    [ProvideToolWindow(typeof(PullRequestsWindow), Style = VsDockStyle.Tabbed, Orientation = ToolWindowOrientation.Left, MultiInstances = false, Transient = true, Window = EnvDTE.Constants.vsWindowKindSolutionExplorer)]
    public sealed class GitClientVSPackage : Package
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public const string ActivityLogName = "BitbucketExtension";
        public const string GitExtensionsId = "11B8E6D7-C08B-4385-B321-321078CDD1F8";

        static GitClientVSPackage()
        {
            AssemblyResolver.InitializeAssemblyResolver();
        }

        public GitClientVSPackage()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        #region Package Members
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error("Unhandled GitClientVsExtensions Error: " + e.ExceptionObject);
        }
        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Error("Unhandled Dispatcher GitClientVsExtensions Error", e.Exception);
        }

        protected override async void Initialize()
        {
            base.Initialize();
            var encoding = new ASCIIEncoding();
            var grdBytes = encoding.GetBytes(_myXaml);
            var xamlstyle = (DrawingBrush)XamlReader.Load(new MemoryStream(grdBytes));
            Application.Current.Resources["ConnectArrowBrush"] = xamlstyle;

            var componentModel = this.GetService<SComponentModel, IComponentModel>();
            var serviceProvider = componentModel.DefaultExportProvider;
            var appInitializer = serviceProvider.GetExportedValue<IAppInitializer>();
            var userService = serviceProvider.GetExportedValue<IUserInformationService>();
            var gitWatcher = serviceProvider.GetExportedValue<IGitWatcher>();
            var commandsService = serviceProvider.GetExportedValue<ICommandsService>();
            commandsService.Initialize(this);
            userService.StartListening();
            gitWatcher.Initialize();
            await appInitializer.Initialize();
            Logger.Info("Initialized GitClientVsPackage Extension");
        }
    //this is a workaround for missing ConnectArrowBrush style from VS
        private readonly String _myXaml =
            @"<DrawingBrush xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <DrawingBrush.Drawing>
      <DrawingGroup>
        <DrawingGroup.Children>
          <GeometryDrawing Brush = ""#004f83"" Geometry=""F1 M 9,11 L 7,11 9,9 4,9 4,7 9,7 7,5 9,5 12,8 Z"" />
          <GeometryDrawing Brush = ""#004f83"" Geometry=""F1 M 7.9741,1.0698 C 4.1461,1.0698 1.0441,4.1728 1.0441,7.9998 1.0441,11.8268 4.1461,14.9298 7.9741,14.9298 11.8011,14.9298 14.9041,11.8268 14.9041,7.9998 14.9041,4.1728 11.8011,1.0698 7.9741,1.0698 M 7.9741,2.0598 C 11.2501,2.0598 13.9151,4.7248 13.9151,7.9998 13.9151,11.2758 11.2501,13.9408 7.9741,13.9408 4.6991,13.9408 2.0341,11.2758 2.0341,7.9998 2.0341,4.7248 4.6991,2.0598 7.9741,2.0598 "" />
        </DrawingGroup.Children>
      </DrawingGroup>
    </DrawingBrush.Drawing>
  </DrawingBrush>";


        #endregion
    }
}
