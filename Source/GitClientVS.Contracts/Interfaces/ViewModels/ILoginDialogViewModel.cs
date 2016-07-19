using System.Windows.Input;

namespace GitClientVS.Contracts.Interfaces.ViewModels
{
    public interface ILoginDialogViewModel : ICloseable
    {
        string Login { get; set; }
        string Password { get; set; }
        string ErrorMessage { get; set; }
        ICommand ConnectCommand { get; }
    }
}