using log4net;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GitClientVS.UI.Helpers
{
    public static class AssemblyResolver
    {
        static bool _resolverInitialized;

        // list of assemblies to be loaded from the extension installation path
        static readonly string[] OurAssemblies =
        {
            "GitClientVS.Contracts",
            "GitClientVS.Infrastructure",
            "GitClientVS.Services",
            "GitClientVS.UI",
            "GitClientVS.VisualStudio.UI",
            "GitClientVS.TeamFoundation.14",
            "GitClientVS.TeamFoundation.15",
            "MahApps.Metro",
            "WpfControls",
            "ICSharpCode.AvalonEdit",
            "ParseDiff",
            "HtmlRenderer",
            "HtmlRenderer.WPF",
            "System.Windows.Interactivity",
            "Humanizer",
            "RestSharp",
            "DiffPlex",
            "System.ValueTuple",
            "Svg",
            "System.Reactive.Core",
            "System.Reactive.Interfaces",
            "System.Reactive.Linq",
            "System.Reactive.PlatformServices",
            "System.Reactive.Windows.Threading",
            "ControlzEx",
            "Markdown.Xaml",
        };//todo version incompability

        public static void InitializeAssemblyResolver()
        {
            Debug.WriteLine("Initializing assembly resolver");

            if (_resolverInitialized)
                return;
            AppDomain.CurrentDomain.AssemblyResolve += LoadAssemblyFromRunDir;
            _resolverInitialized = true;

            LoadAssembly("MahApps.Metro"); // many issues with unloaded mahapps.metro
            LoadAssembly("ControlzEx");
        }

        public static Assembly LoadAssembly(string assemblyName)
        {
            Debug.WriteLine($"Loading assembly: {assemblyName}");

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = Path.Combine(path, assemblyName + ".dll");

            if (!File.Exists(filename))
            {
                Debug.WriteLine($"Couldn't load assembly {assemblyName} from {Path.GetFullPath(filename)}");
                return null;
            }

            return Assembly.LoadFrom(filename);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods")]
        static Assembly LoadAssemblyFromRunDir(object sender, ResolveEventArgs e)
        {
            try
            {
                var name = new AssemblyName(e.Name);
                if (!OurAssemblies.Contains(name.Name))
                    return null;

                Debug.WriteLine("Loading unloaded assembly from assembly resolve");

                return LoadAssembly(name.Name);
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

                Debug.WriteLine(log);
            }
            return null;
        }
    }
}