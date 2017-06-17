using System;
using System.ComponentModel.Composition;
using System.IO;
using EnvDTE;
using GitClientVS.Contracts.Interfaces.Services;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace GitClientVS.VisualStudio.UI.Services
{
    [Export(typeof(IVsTools))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class VsTools : IVsTools
    {
        private readonly IAppServiceProvider _appServiceProvider;
        private const string TempSolutionName = "GitClientTempSolution";
        private IFileService _fileService;

        [ImportingConstructor]
        public VsTools(IAppServiceProvider appServiceProvider, IFileService fileService)
        {
            _appServiceProvider = appServiceProvider;
            _fileService = fileService;
        }

        public void OpenTemporarySolution(string repositoryPath) 
            //this is Github extension idea to change active repository from TeamExplorer Window.
        {
            var dte = _appServiceProvider.GetService<DTE>();

            if (dte == null)
                throw new Exception("DTE not found");

            if (!_fileService.Exists(repositoryPath))
                throw new Exception($"Repo path doesn't exist: {repositoryPath}");

            try
            {
                dte.Solution.Create(repositoryPath, TempSolutionName);
                dte.Solution.Close(false);
            }
            finally
            {
                DeleteVsDir(repositoryPath);
            }
        }

        public bool OpenSolutionViaDlg(string path)
        {
            var solutionService = (IVsSolution)_appServiceProvider.GetService<SVsSolution>();
            return ErrorHandler.Succeeded(solutionService.OpenSolutionViaDlg(path, 1));
        }

        private void DeleteVsDir(string repositoryPath)
        {
            try
            {
                _fileService.DeleteDirectory(Path.Combine(repositoryPath, ".vs", TempSolutionName));
            }
            catch (Exception) { }//ignore errors here 
        }
    }
}
