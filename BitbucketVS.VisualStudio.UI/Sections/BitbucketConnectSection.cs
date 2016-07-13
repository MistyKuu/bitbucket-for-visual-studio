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

namespace BitbucketVS.VisualStudio.UI.Sections
{
    [TeamExplorerSection(BitbucketConnectSectionId, TeamExplorerPageIds.Connect, 10)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BitbucketConnectSection : TeamExplorerBaseSection
    {
        private ITeamExplorerSection _section;
        public const string BitbucketConnectSectionId = "a6701970-28da-42ee-a0f4-9e02f486de2c";

        protected BitbucketConnectSectionView View
        {
            get { return SectionContent as BitbucketConnectSectionView; }
            set { SectionContent = value; }
        }

        public BitbucketConnectSection()
        {
            Title = "BitBucketExt1";
            PropertyChanged += BitbucketConnectSection_PropertyChanged;
        }

        private void BitbucketConnectSection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsVisible" && IsVisible && View == null)
                View = new BitbucketConnectSectionView { DataContext = this };
            //else if (e.PropertyName == "IsExpanded" && settings != null)
            //    settings.IsExpanded = IsExpanded;
        }

        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);

            // watch for new repos added to the local repo list
            _section = GetSection(TeamExplorerConnectionsSectionId);
        }

        protected ITeamExplorerSection GetSection(Guid section)
        {
            return ((ITeamExplorerPage) ServiceProvider.GetService(typeof(ITeamExplorerPage))).GetSection(section);
        }

    }
}
