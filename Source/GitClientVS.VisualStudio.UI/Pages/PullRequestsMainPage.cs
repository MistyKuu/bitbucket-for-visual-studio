using System.ComponentModel.Composition;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.UI;
using GitClientVS.VisualStudio.UI.TeamFoundation;
using Microsoft.TeamFoundation.Controls;

namespace GitClientVS.VisualStudio.UI.Pages
{
    [TeamExplorerPage(PageId)]
    public class PullRequestsMainPage : TeamExplorerBasePage
    {
        public const string PageId = "8681fe34-eb88-49c8-b848-f1becb636213";

        [ImportingConstructor]
        public PullRequestsMainPage(IPullRequestsMainView view)
        {
            Title = Resources.PullRequestNavigationItemTitle;
            PageContent = view;
        }
    }
}
