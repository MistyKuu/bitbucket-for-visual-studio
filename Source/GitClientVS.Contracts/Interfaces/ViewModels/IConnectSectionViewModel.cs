using System;

namespace GitClientVS.Contracts.Interfaces.ViewModels
{
    public interface IConnectSectionViewModel : IViewModel, IViewModelWithCommands, IViewModelWithErrorMessage, IInitializable, IDisposable
    {
        void ChangeActiveRepo();
    }
}
