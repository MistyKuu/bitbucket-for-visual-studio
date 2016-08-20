using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Infrastructure.Extensions;
using Microsoft.TeamFoundation.Controls;
using NotificationFlags = GitClientVS.Contracts.NotificationFlags;
using NotificationType = GitClientVS.Contracts.NotificationType;

namespace GitClientVS.VisualStudio.UI.Services
{
    [Export(typeof(IVsTools))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class VsTools : IVsTools
    {
        private readonly IAppServiceProvider _appServiceProvider;
        private DTE _dte;
        private ITeamExplorer _teamExplorer;

        private ITeamExplorer TeamExplorer => (_teamExplorer = _teamExplorer ?? _appServiceProvider.GetService<ITeamExplorer>());

        [ImportingConstructor]
        public VsTools(IAppServiceProvider appServiceProvider)
        {
            _appServiceProvider = appServiceProvider;
        }

        public void RunDiff(string file1, string file2)
        {
            _dte = (DTE)_appServiceProvider.GetService(typeof(DTE));
            _dte.ExecuteCommand("Tools.DiffFiles", $"\"{file1}\" \"{file2}\"");
        }
    }
}
