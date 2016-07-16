using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.UI.Views;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.MVVM;
using GitClientVS.VisualStudio.UI.TeamFoundation;
using GitClientVS.Contracts;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Infrastructure;
using Reactive.EventAggregator;

namespace GitClientVS.VisualStudio.UI.Sections
{
    [TeamExplorerSection(Id, TeamExplorerPageIds.Connect, 10)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConnectSection : TeamExplorerBaseSection
    {
        private ITeamExplorerSection _section;
        private const string Id = "a6701970-28da-42ee-a0f4-9e02f486de2c";

        [ImportingConstructor]
        public ConnectSection(
            IBitbucketService bucketService,
            IConnectSectionView sectionView) : base(sectionView)
        {
            LoggerConfigurator.Setup(); // TODO this needs to be set in the entry point like package
            Title = "Bitbucket Extension";
        }

        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);

            // watch for new repos added to the local repo list
            _section = GetSection(TeamExplorerConnectionsSectionId);

        }

        protected ITeamExplorerSection GetSection(Guid section)
        {
            return ((ITeamExplorerPage)ServiceProvider.GetService(typeof(ITeamExplorerPage))).GetSection(section);
        }

    }
}
