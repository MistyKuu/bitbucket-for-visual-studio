using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces.Services;
using log4net;
using Microsoft.TeamFoundation.Git.Controls.Extensibility;

namespace GitClientVS.VisualStudio.UI.Services
{
    [Export(typeof(IGitService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GitService: IGitService
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IAppServiceProvider _appServiceProvider; 

        [ImportingConstructor]
        public GitService(IAppServiceProvider appServiceProvider)
        {
            _appServiceProvider = appServiceProvider;
        }

        public void CloneRepository(string cloneUrl, string repositoryName, string repositoryPath)
        {
            var gitExt = _appServiceProvider.GetService<IGitRepositoriesExt>();
            string path = Path.Combine(repositoryPath, repositoryName);

            Directory.CreateDirectory(path);

            try
            {
                gitExt.Clone(cloneUrl, path, CloneOptions.None);
            }
            catch (Exception ex)
            {
                Logger.Error($"Could not clone {cloneUrl} to {path}. {ex}");
                throw;
            }
        }


    }
}
