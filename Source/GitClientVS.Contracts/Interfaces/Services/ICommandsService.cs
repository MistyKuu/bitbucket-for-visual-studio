
namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface ICommandsService
    {
        void ShowDiffWindow(object parameter, int id);
        void Initialize(object package);
    }
}