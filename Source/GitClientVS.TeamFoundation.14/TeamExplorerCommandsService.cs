using GitClientVS.Contracts.Interfaces.Services;
using Microsoft.TeamFoundation.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;

namespace GitClientVS.TeamFoundation
{
    [Export(typeof(ITeamExplorerCommandsService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class TeamExplorerCommandsService : ITeamExplorerCommandsService
    {
        private readonly ITeamExplorer _teamExplorer; // good morning MEF, I'm empty field but if I'm not here you won't load my plugin silently :)
        private readonly IAppServiceProvider _appServiceProvider;

        [ImportingConstructor]
        public TeamExplorerCommandsService(IAppServiceProvider appServiceProvider)
        {
            _appServiceProvider = appServiceProvider;
        }

        public void NavigateToHomePage()
        {
            _appServiceProvider.GetService<ITeamExplorer>()?.NavigateToPage(new Guid(TeamExplorerPageIds.Home), null);
        }
    }
}
