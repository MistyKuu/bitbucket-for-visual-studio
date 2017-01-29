using System;
using System.ComponentModel.Composition;
using System.Reactive;
using System.Reactive.Linq;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Models;
using GitClientVS.TeamFoundation.TeamFoundation;
using GitClientVS.UI;
using Microsoft.TeamFoundation.Controls;

namespace GitClientVS.TeamFoundation.NavigationItems
{
    [TeamExplorerNavigationItem(PullRequestsNavigationItemId, 320)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PullRequestNavigationItem : TeamExplorerBaseNavigationItem
    {
        private readonly IGitClientService _gitClientService;
        private readonly IGitService _gitService;
        private readonly IEventAggregatorService _eventAggregator;
        private readonly IUserInformationService _userInformationService;
        private readonly ICommandsService _commandService;
        private IDisposable _observable;
        public const string PullRequestsNavigationItemId = "5245767A-B657-4F8E-BFEE-F04159F1DDA3";

        [ImportingConstructor]
        public PullRequestNavigationItem(
            IGitClientService gitClientService,
            IGitService gitService,
            IEventAggregatorService eventAggregator,
            IUserInformationService userInformationService,
            ICommandsService commandService
            ) : base(null)
        {
            _gitClientService = gitClientService;
            _gitService = gitService;
            _eventAggregator = eventAggregator;
            _userInformationService = userInformationService;
            _commandService = commandService;
            Text = Resources.PullRequestNavigationItemTitle;
            Image = Resources.PullRequest;
            IsVisible = ShouldBeVisible(_userInformationService.ConnectionData);
            var connectionObs = _eventAggregator.GetEvent<ConnectionChangedEvent>();
            var repoObs = _eventAggregator.GetEvent<ActiveRepositoryChangedEvent>();

            _observable = connectionObs.Select(x => Unit.Default).Merge(repoObs.Select(x => Unit.Default)).Subscribe(_ => ValidateVisibility());
        }

        private void ValidateVisibility()
        {
            IsVisible = ShouldBeVisible(_userInformationService.ConnectionData);
        }


        private bool ShouldBeVisible(ConnectionData connectionData)
        {
            return connectionData.IsLoggedIn && _gitClientService.IsOriginRepo(_gitService.GetActiveRepository());
        }

        public override void Execute()
        {
            _commandService.ShowPullRequestsWindow();
        }

        public override void Dispose()
        {
            _observable.Dispose();
        }
    }
}
