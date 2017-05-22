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

        public void RunDiff(string content1, string content2, string fileDisplayName1, string fileDisplayName2)
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "GitClientVs");
            Directory.CreateDirectory(tempDir);

            var tempPath1 = Path.Combine(tempDir, Guid.NewGuid().ToString());
            var tempPath2 = Path.Combine(tempDir, Guid.NewGuid().ToString());

            File.WriteAllText(tempPath1, content1);
            File.WriteAllText(tempPath2, content2);

            var differenceService = Package.GetGlobalService(typeof(SVsDifferenceService)) as IVsDifferenceService;

            var options = __VSDIFFSERVICEOPTIONS.VSDIFFOPT_DetectBinaryFiles |
                          __VSDIFFSERVICEOPTIONS.VSDIFFOPT_LeftFileIsTemporary |
                          __VSDIFFSERVICEOPTIONS.VSDIFFOPT_RightFileIsTemporary;

            differenceService.OpenComparisonWindow2(
                tempPath1,
                tempPath2,
                "captionmy",
                "mytooltip",
                fileDisplayName1,
                fileDisplayName2,
                "inlinelabel",
                "roles",
                (uint)options
            );

            File.Delete(tempPath1);
            File.Delete(tempPath2);

            // _dte = (DTE)_appServiceProvider.GetService(typeof(DTE));
            // _dte.ExecuteCommand("Tools.DiffFiles", $"\"{file1}\" \"{content2}\" \"{fileDisplayName1}\" \"{fileDisplayName2}\"");

        }
    }
}
