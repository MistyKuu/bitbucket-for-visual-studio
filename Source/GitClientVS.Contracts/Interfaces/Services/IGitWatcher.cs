using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IGitWatcher
    {
        GitRepository ActiveRepo { get; }
        void Initialize();
        void Refresh();
    }
}