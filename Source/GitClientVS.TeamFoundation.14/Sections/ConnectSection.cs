using System;
using System.ComponentModel.Composition;
using System.Reflection;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.TeamFoundation.TeamFoundation;
using log4net;
using Microsoft.TeamFoundation.Controls;

namespace GitClientVS.TeamFoundation.Sections
{
    [TeamExplorerSection(Id, TeamExplorerPageIds.Connect, 20)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConnectSection : TeamExplorerBaseSection
    {
        private readonly IAppServiceProvider _appServiceProvider;
        private ITeamExplorerSection _section;
        private const string Id = "a6701970-28da-42ee-a0f4-9e02f486de2c";
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [ImportingConstructor]
        public ConnectSection(
            IGitClientService bucketService,
            IAppServiceProvider appServiceProvider,
            IGitClientService gitClient,
            IConnectSectionView sectionView) : base(sectionView)
        {
            _appServiceProvider = appServiceProvider;
            Title = gitClient.Title;
        }



        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            _appServiceProvider.GitServiceProvider = ServiceProvider = e.ServiceProvider;
            _section = GetSection(TeamExplorerConnectionsSectionId);
            base.Initialize(sender, e);
        }


        protected ITeamExplorerSection GetSection(Guid section)
        {
            return ((ITeamExplorerPage)ServiceProvider.GetService(typeof(ITeamExplorerPage))).GetSection(section);
        }



    }
}
