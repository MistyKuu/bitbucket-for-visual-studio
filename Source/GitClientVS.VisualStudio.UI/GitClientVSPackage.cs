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
using System.Windows;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Infrastructure.Extensions;
using GitClientVS.VisualStudio.UI.Window;
using log4net;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Git.Controls.Extensibility;
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
    [Guid(GitClientVSPackage.PackageGuidString)]
    [ProvideAutoLoad(GitExtensionsId)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(DiffWindow), Style = VsDockStyle.MDI, Orientation = ToolWindowOrientation.Left, MultiInstances = true, Transient = true)]
    public sealed class GitClientVSPackage : Package
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public const string PackageGuidString = "69c97fa4-92b5-448c-b5db-037dd9c2c8b7";
        public const string GitExtensionsId = "11B8E6D7-C08B-4385-B321-321078CDD1F8";

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
            AppDomain.CurrentDomain.AssemblyResolve += LoadNotLoadedAssemblies;
            base.Initialize();

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

        #region JustInCaseLoadingAssemblies

        static readonly string[] OurAssemblies =
      {
            "GitClientVS.Api",
            "GitClientVS.Contracts",
            "GitClientVS.Infrastructure",
            "GitClientVS.Services",
            "GitClientVS.UI",
            "GitClientVS.VisualStudio.UI",
            "MahApps.Metro",
            "WpfControls"
        };


        private Assembly LoadNotLoadedAssemblies(object sender, ResolveEventArgs e)
        {
            try
            {
                var name = new AssemblyName(e.Name);
                if (!OurAssemblies.Contains(name.Name))
                    return null;
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var filename = Path.Combine(path, name.Name + ".dll");
                if (!File.Exists(filename))
                    return null;
                return Assembly.LoadFrom(filename);
            }
            catch (Exception ex)
            {
                var log = string.Format(CultureInfo.CurrentCulture,
                    "Error occurred loading {0} from {1}.{2}{3}{4}",
                    e.Name,
                    Assembly.GetExecutingAssembly().Location,
                    Environment.NewLine,
                    ex,
                    Environment.NewLine);

                Logger.Error(log);

            }
            return null;
        }
        #endregion

        #endregion
    }
}
