using System;
using System.ComponentModel.Composition;
using System.IO;
using EnvDTE;
using GitClientVS.Contracts.Interfaces.Services;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace GitClientVS.VisualStudio.UI.Services
{
    [Export(typeof(IVsTools))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class VsTools : IVsTools
    {
        private readonly IAppServiceProvider _appServiceProvider;
        private DTE _dte;

        [ImportingConstructor]
        public VsTools(IAppServiceProvider appServiceProvider)
        {
            _appServiceProvider = appServiceProvider;
        }

        public void RunDiff(
            string content1,
            string content2,
            string fileDisplayName1,
            string fileDisplayName2,
            string caption,
            string tooltip
            )
        {

            (string file1, string file2) = CreateTempFiles(content1, content2);

            try
            {
                var differenceService = Package.GetGlobalService(typeof(SVsDifferenceService)) as IVsDifferenceService;

                var options = __VSDIFFSERVICEOPTIONS.VSDIFFOPT_DetectBinaryFiles |
                              __VSDIFFSERVICEOPTIONS.VSDIFFOPT_LeftFileIsTemporary |
                              __VSDIFFSERVICEOPTIONS.VSDIFFOPT_RightFileIsTemporary;

                differenceService.OpenComparisonWindow2(
                    file1,
                    file2,
                    caption,
                    tooltip,
                    fileDisplayName1,
                    fileDisplayName2,
                    null,
                    null,
                    (uint)options
                );
            }
            finally
            {
                if (File.Exists(file1))
                    File.Delete(file1);
                if (File.Exists(file2))
                    File.Delete(file2);
            }
        }

        private static (string file1, string file2) CreateTempFiles(string content1, string content2)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "GitClientVs");
            Directory.CreateDirectory(tempDir);

            var tempPath1 = Path.Combine(tempDir, Guid.NewGuid().ToString());
            var tempPath2 = Path.Combine(tempDir, Guid.NewGuid().ToString());

            File.WriteAllText(tempPath1, content1);
            File.WriteAllText(tempPath2, content2);

            return (tempPath1, tempPath2);
        }
    }
}
