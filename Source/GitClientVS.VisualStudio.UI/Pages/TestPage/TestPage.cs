using GitClientVS.VisualStudio.UI.TeamFoundation;
using Microsoft.TeamFoundation.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.VisualStudio.UI.Pages.TestPage
{
    [TeamExplorerPage(TestPage.PageId)]
    public class TestPage : TeamExplorerBasePage
    {
        public const string PageId = "BAC5373E-1BE5-4A10-97F5-AC278CA77EDF";

        public TestPage()
        {
            // Set the page title
            this.Title = "My History";
            this.PageContent = new TestPageView();
            this.View.ParentSection = this;
        }

        protected TestPageView View
        {
            get { return this.PageContent as TestPageView; }
        }
    }
}
