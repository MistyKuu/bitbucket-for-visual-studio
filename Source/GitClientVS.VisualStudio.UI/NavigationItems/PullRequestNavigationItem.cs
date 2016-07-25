using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
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
        public const string PullRequestsNavigationItemId = "4269bd3b-7f80-4463-978e-8a1c9431c362";

        [ImportingConstructor]
        public PullRequestNavigationItem([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Text = Resources.PullRequestNavigationItemTitle;
            Image = Resources.luki;
            //TODO theme changed
            // Is BitBucketRepo otherwise don't show
        }

        public override void Execute()
        {
            var service = this.GetService<ITeamExplorer>();
            service?.NavigateToPage(new Guid(PullRequestsMainPage.PageId), null);
        }
    }
}
