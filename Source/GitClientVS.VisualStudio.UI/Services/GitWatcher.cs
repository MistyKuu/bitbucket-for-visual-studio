using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Events;
using GitClientVS.VisualStudio.UI.Extensions;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;

namespace GitClientVS.VisualStudio.UI.Services
{
    [Export(typeof(IGitWatcher))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GitWatcher : IGitWatcher
    {
        private readonly SynchronizationContext syncContext;
        private readonly IGitExt gitExt;
        private readonly IEventAggregatorService eventAggregatorService;

        [ImportingConstructor]
        public GitWatcher(
            IAppServiceProvider appServiceProvider,
            IEventAggregatorService eventAggregatorService)
        {
            syncContext = SynchronizationContext.Current;
            this.eventAggregatorService = eventAggregatorService;
            gitExt = appServiceProvider.GetService<IGitExt>();

            gitExt.PropertyChanged += CheckAndUpdate;
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
                if (activeRepo == value)
                    return;
   
                activeRepo = value;

                eventAggregatorService.Publish(new ActiveRepositoryChangedEvent(value));
            }
        }
    }
}