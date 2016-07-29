using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GitClientVS.UI.Views;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.MVVM;
using GitClientVS.VisualStudio.UI.TeamFoundation;
using GitClientVS.Contracts;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models;
using GitClientVS.Infrastructure;
using Reactive.EventAggregator;
using GitClientVS.Infrastructure.Extensions;
using GitClientVS.Services;
using log4net;

namespace GitClientVS.VisualStudio.UI.Sections
{
    [TeamExplorerSection(Id, TeamExplorerPageIds.Connect, 10)]
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
