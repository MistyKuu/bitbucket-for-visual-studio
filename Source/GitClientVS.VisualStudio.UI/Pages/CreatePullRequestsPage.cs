using System.ComponentModel.Composition;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Infrastructure;
using GitClientVS.UI;
using GitClientVS.VisualStudio.UI.TeamFoundation;
using Microsoft.TeamFoundation.Controls;

namespace GitClientVS.VisualStudio.UI.Pages
{
    [TeamExplorerPage(PageIds.CreatePullRequestsPageId)]
    public class CreatePullRequestsPage : TeamExplorerBasePage
    {
        [ImportingConstructor]
        public CreatePullRequestsPage(ICreatePullRequestsView view)
        {
            Title = Resources.CreatePullRequestsPageTitle;
            PageContent = view;
        }

    }
}
