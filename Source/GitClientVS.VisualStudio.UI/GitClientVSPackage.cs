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
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Infrastructure;
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
using Task = System.Threading.Tasks.Task;

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
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(GuidList.guidBitbuketPkgString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
         Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    [ProvideToolWindow(typeof(DiffWindow), Style = VsDockStyle.MDI, Orientation = ToolWindowOrientation.Left,
         MultiInstances = true, Transient = true)]
    [ProvideToolWindow(typeof(PullRequestsWindow), Style = VsDockStyle.Tabbed, Orientation = ToolWindowOrientation.Left,
         MultiInstances = false, Transient = true, Window = EnvDTE.Constants.vsWindowKindSolutionExplorer)]
    public sealed class GitClientVSPackage : AsyncPackage
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

        private void Current_DispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Error("Unhandled Dispatcher GitClientVsExtensions Error", e.Exception);
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);
            var componentModel = (IComponentModel)await GetServiceAsync(typeof(SComponentModel));

            await InitializePackageAsync(componentModel);
        }

        private async Task InitializePackageAsync(IComponentModel componentModel)
        {
            try
            {
                var serviceProvider = componentModel.DefaultExportProvider;

                Application.Current.Resources.Add(Consts.IocResource, serviceProvider);

                var appInitializer = serviceProvider.GetExportedValue<IAppInitializer>();
                var commandsService = serviceProvider.GetExportedValue<ICommandsService>();
                var gitWatcher = serviceProvider.GetExportedValue<IGitWatcher>();
                commandsService.Initialize(this);
                gitWatcher.Initialize();
                await appInitializer.Initialize();

                Logger.Info("Initialized GitClientVsPackage Extension");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error Loading GitClientVsExtensionPackage: {ex}");
            }
        }

        #endregion
    }
}
