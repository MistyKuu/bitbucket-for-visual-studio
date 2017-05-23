
using ParseDiff;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface ICommandsService
    {
        void ShowDiffWindow(FileDiff parameter, int id);
        void Initialize(object package);
        void ShowPullRequestsWindow();
    }
}