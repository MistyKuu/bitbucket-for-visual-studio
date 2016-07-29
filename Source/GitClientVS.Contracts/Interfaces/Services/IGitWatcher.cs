using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IGitWatcher
    {
        GitRemoteRepository ActiveRepo { get; }
    }
}