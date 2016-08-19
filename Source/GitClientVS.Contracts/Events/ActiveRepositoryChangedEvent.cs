using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Contracts.Events
{
    public class ActiveRepositoryChangedEvent
    {
        public GitRemoteRepository ActiveRepository { get; set; }

        public ActiveRepositoryChangedEvent(GitRemoteRepository activeRepository)
        {
            ActiveRepository = activeRepository;
        }
    }
}