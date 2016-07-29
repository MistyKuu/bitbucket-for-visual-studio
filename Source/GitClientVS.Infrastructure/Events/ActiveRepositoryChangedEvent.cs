using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Infrastructure.Events
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