using System;
using System.ComponentModel.Composition;
using System.Reflection;
using Microsoft.TeamFoundation.Controls;
using GitClientVS.VisualStudio.UI.TeamFoundation;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.Views;
using log4net;

namespace GitClientVS.VisualStudio.UI.Sections
{
    [TeamExplorerSection(Id, TeamExplorerPageIds.Connect, 20)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConnectSection : TeamExplorerBaseSection
    {
        private readonly IAppServiceProvider _appServiceProvider;
        private const string Id = "a6701970-28da-42ee-a0f4-9e02f486de2c";
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [ImportingConstructor]
        public ConnectSection(
            IAppServiceProvider appServiceProvider,
            IConnectSectionView sectionView) : base(sectionView)
        {
            _appServiceProvider = appServiceProvider;
            Title = "Git Client Extensions";
        }


        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            _appServiceProvider.GitServiceProvider = ServiceProvider = e.ServiceProvider;
            base.Initialize(sender, e);
        }


    }
}
