using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Events;
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
        private readonly IAppServiceProvider _appServiceProvider;
        private readonly IGitWatcher _gitWatcher;
        private readonly IEventAggregatorService _eventAggregator;
        private IDisposable _obs;
        private const string Id = "8a950046-66b6-4607-9038-4d0b7eb8ab96";

        [ImportingConstructor]
        public PublishSection(
            IPublishSectionView view,
            IAppServiceProvider appServiceProvider,
            IGitClientService gitClientService,
            IGitWatcher gitWatcher,
            IEventAggregatorService eventAggregator
            ) : base(view)
        {
            _appServiceProvider = appServiceProvider;
            _eventAggregator = eventAggregator;
            Title = $"{Resources.PublishSectionTitle} to {gitClientService.Origin}";
            IsVisible = IsGitLocalRepo(gitWatcher.ActiveRepo);
            _obs = _eventAggregator.GetEvent<ActiveRepositoryChangedEvent>().Subscribe(x => IsVisible = IsGitLocalRepo(x.ActiveRepository));
        }

        private bool IsGitLocalRepo(GitRemoteRepository repo)
        {
            return (repo != null && string.IsNullOrEmpty(repo.CloneUrl));
        }

        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            _appServiceProvider.GitServiceProvider = ServiceProvider = e.ServiceProvider;
            base.Initialize(sender, e);
        }

        public override void Dispose()
        {
            base.Dispose();
            _obs.Dispose();
        }
    }
}
