using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.UI;
using GitClientVS.VisualStudio.UI.TeamFoundation;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;

namespace GitClientVS.VisualStudio.UI.Sections
{
    [TeamExplorerSection(Id, TeamExplorerPageIds.GitCommits, 50)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PublishSection : TeamExplorerBaseSection
    {
        private const string Id = "8a950046-66b6-4607-9038-4d0b7eb8ab96";

        [ImportingConstructor]
        public PublishSection(IPublishSectionView view) : base(view)
        {
            Title = Resources.PublishSectionTitle;
        }
    }
}
