using System.Windows.Input;

namespace GitClientVS.Contracts.Interfaces.ViewModels
{
    public interface ILoginDialogViewModel : ICloseable, IViewModelWithErrorMessage, ILoadableViewModel
    {
        string Login { get; set; }
        string Password { get; set; }
        ICommand ConnectCommand { get; }
    }
}