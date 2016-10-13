using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Contracts.Events
{
    public class ActiveRepositoryChangedEvent
    {
        public GitRepository ActiveRepository { get; set; }

        public ActiveRepositoryChangedEvent(GitRepository activeRepository)
        {
            ActiveRepository = activeRepository;
        }
    }
}