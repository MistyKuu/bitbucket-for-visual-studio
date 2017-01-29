using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.TeamFoundation.Extensions;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;

namespace GitClientVS.TeamFoundation
{
    [Export(typeof(IGitWatcher))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GitWatcher : IGitWatcher
    {
        private readonly SynchronizationContext syncContext;
        private readonly IGitExt gitExt;
        private readonly IEventAggregatorService eventAggregatorService;
        private readonly IGitService _gitService;

        [ImportingConstructor]
        public GitWatcher(
            IAppServiceProvider appServiceProvider,
            IEventAggregatorService eventAggregatorService,
            IGitService gitService
            )
        {
            syncContext = SynchronizationContext.Current;
            this.eventAggregatorService = eventAggregatorService;
            _gitService = gitService;
            gitExt = appServiceProvider.GetService<IGitExt>();
        }

        public void Initialize()
        {
            ActiveRepo = _gitService.GetActiveRepository();
            gitExt.PropertyChanged += CheckAndUpdate;
        }

        public void Refresh()
        {
            ActiveRepo = _gitService.GetActiveRepository();
        }

        void CheckAndUpdate(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "ActiveRepositories")
                return;

            var repo = gitExt.ActiveRepositories.FirstOrDefault()?.ToModel();
            if (repo != ActiveRepo)
                syncContext.Post(r => ActiveRepo = r as GitRemoteRepository, repo);
        }


        private GitRemoteRepository activeRepo;
        public GitRemoteRepository ActiveRepo
        {
            get { return activeRepo; }
            private set
            {
                activeRepo = value;

                eventAggregatorService.Publish(new ActiveRepositoryChangedEvent(value));
            }
        }
    }
}