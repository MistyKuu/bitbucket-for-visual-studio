using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Drawing;
using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell;
using BitbucketVS.VisualStudio.UI.TeamFoundation;
using BitbucketVS.VisualStudio.UI.Pages.TestPage;

namespace BitbucketVS.VisualStudio.UI
{
    [TeamExplorerNavigationItem(TestNavigationItem.LinkId, 200, TargetPageId = TestNavigationItem.LinkId)]
    public class TestNavigationItem : TeamExplorerBaseNavigationItem
    {
        public const string LinkId = "e49a882b-1677-46a9-93b4-db290943bbcd";

        [ImportingConstructor]
        public TestNavigationItem([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.Text = "My History";
            if (this.CurrentContext != null && this.CurrentContext.HasCollection && this.CurrentContext.HasTeamProject)
            {
                this.IsVisible = true;
            }

            Image bmp = Image.FromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.ALMRangers.Samples.MyHistory.Resources.MyHistory.png"));
            this.Image = bmp;
        }

        public override void Execute()
        {
            try
            {
                ITeamExplorer teamExplorer = GetService<ITeamExplorer>();
                if (teamExplorer != null)
                {
                    teamExplorer.NavigateToPage(new Guid(TestPage.PageId), null);
                }
            }
            catch (Exception ex)
            {
                this.ShowNotification(ex.Message, NotificationType.Error);
            }
        }

        public override void Invalidate()
        {
            base.Invalidate();
            if (this.CurrentContext != null && this.CurrentContext.HasCollection && this.CurrentContext.HasTeamProject)
            {
                this.IsVisible = true;
            }
            else
            {
                this.IsVisible = false;
            }
        }
    }
}
