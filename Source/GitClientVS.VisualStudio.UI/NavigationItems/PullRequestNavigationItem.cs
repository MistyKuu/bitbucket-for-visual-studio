using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models;
using GitClientVS.Infrastructure.Events;
using GitClientVS.UI;
using GitClientVS.VisualStudio.UI.Pages;
using GitClientVS.VisualStudio.UI.TeamFoundation;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;
using Microsoft.VisualStudio.Shell;

namespace GitClientVS.VisualStudio.UI.NavigationItems
{
    [TeamExplorerNavigationItem(PullRequestsNavigationItemId, 1)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PullRequestNavigationItem : TeamExplorerBaseNavigationItem
    {
        private readonly IPageNavigationService _navigationService;
        private readonly IGitClientService _gitClientService;
        private readonly IGitService _gitService;
        private readonly IUserInformationService _userInfoService;
        private readonly IEventAggregatorService _eventAggregator;
        private readonly IUserInformationService _userInformationService;
        private IDisposable _observable;
        public const string PullRequestsNavigationItemId = "4269bd3b-7f80-4463-978e-8a1c9431c362";

        [ImportingConstructor]
        public PullRequestNavigationItem(
            IPageNavigationService navigationService,
            IGitClientService gitClientService,
            IGitService gitService,
            IEventAggregatorService eventAggregator,
            IUserInformationService userInformationService
            ) : base(null)
        {
            _navigationService = navigationService;
            _gitClientService = gitClientService;
            _gitService = gitService;
            _eventAggregator = eventAggregator;
            _userInformationService = userInformationService;
            Text = Resources.PullRequestNavigationItemTitle;
            Image = Resources.luki;
           // IsVisible = ShouldBeVisible(_userInformationService.ConnectionData);

            _observable = _eventAggregator.GetEvent<ConnectionChangedEvent>().Subscribe(ConnectionChanged);
        }

        private void ConnectionChanged(ConnectionChangedEvent connectionChangedEvent)
        {
            IsVisible = ShouldBeVisible(connectionChangedEvent.Data);
        }

        private bool ShouldBeVisible(ConnectionData connectionData)
        {
            return _gitClientService.IsOriginRepo(_gitService.GetActiveRepository()) && connectionData.IsLoggedIn;
        }

        public override void Execute()
        {
            _navigationService.Navigate(PullRequestsMainPage.PageId);
        }

        public override void Dispose()
        {
            _observable.Dispose();
        }
    }
}
