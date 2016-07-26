using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using GitClientVS.Contracts.Interfaces.Services;
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
        public const string PullRequestsNavigationItemId = "4269bd3b-7f80-4463-978e-8a1c9431c362";

        [ImportingConstructor]
        public PullRequestNavigationItem(IPageNavigationService navigationService) : base(null)
        {
            _navigationService = navigationService;
            Text = Resources.PullRequestNavigationItemTitle;
            Image = Resources.luki;
            //TODO theme changed
            // Is BitBucketRepo otherwise don't show
        }

        public override void Execute()
        {
            _navigationService.Navigate(PullRequestsMainPage.PageId);
        }
    }
}
