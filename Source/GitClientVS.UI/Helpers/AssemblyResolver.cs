using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GitClientVS.UI.Helpers
{
    public static class AssemblyResolver
    {
        static bool resolverInitialized;

        // list of assemblies to be loaded from the extension installation path
        static readonly string[] OurAssemblies =
        {
            "GitClientVS.Api",
            "GitClientVS.Contracts",
            "GitClientVS.Infrastructure",
            "GitClientVS.Services",
            "GitClientVS.UI",
            "GitClientVS.VisualStudio.UI",
            "MahApps.Metro",
            "WpfControls",
            "ICSharpCode.AvalonEdit",
            "ParseDiff",
            "HtmlRenderer",
            "HtmlRenderer.WPF",
            "System.Windows.Interactivity",
            "Humanizer"
        };

        public static void InitializeAssemblyResolver()
        {
            if (resolverInitialized)
                return;
            AppDomain.CurrentDomain.AssemblyResolve += LoadAssemblyFromRunDir;
            resolverInitialized = true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods")]
        static Assembly LoadAssemblyFromRunDir(object sender, ResolveEventArgs e)
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
               
            }
            return null;
        }
    }
}