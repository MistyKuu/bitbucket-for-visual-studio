using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitbucketVS.UI.Views;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.MVVM;
using BitbucketVS.VisualStudio.UI.TeamFoundation;
using BitBucketVs.Contracts;
using BitBucketVs.Contracts.Interfaces.ViewModels;
using BitBucketVs.Contracts.Interfaces.Views;

namespace BitbucketVS.VisualStudio.UI.Sections
{
    [TeamExplorerSection(Id, TeamExplorerPageIds.Connect, 10)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BitbucketConnectSection : TeamExplorerBaseSection
    {
        private ITeamExplorerSection _section;
        private IDialogWindow _dialogWindow;
        private const string Id = "a6701970-28da-42ee-a0f4-9e02f486de2c";

        [ImportingConstructor]
        public BitbucketConnectSection(
            IBitbucketService bucketService,
            IBitbucketConnectViewModel viewModel,
            IBitbucketConnectView view,
            ITestDialogWindow dialogWindow
            ) : base(viewModel, view)
        {
            Title = "Bitbucket Extensions";
            _dialogWindow = dialogWindow;
        }

        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            // watch for new repos added to the local repo list
            _section = GetSection(TeamExplorerConnectionsSectionId);

           // _dialogWindow.ShowModal();
        }

        protected ITeamExplorerSection GetSection(Guid section)
        {
            return ((ITeamExplorerPage)ServiceProvider.GetService(typeof(ITeamExplorerPage))).GetSection(section);
        }

    }
}
