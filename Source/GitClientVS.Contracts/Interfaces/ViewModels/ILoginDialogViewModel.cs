using System.Windows.Input;

namespace GitClientVS.Contracts.Interfaces.ViewModels
{
    public interface ILoginDialogViewModel
    {
        string Login { get; set; }
        string Password { get; set; }
        string Error { get; set; }
        ICommand ConnectCommand { get; }
    }
}