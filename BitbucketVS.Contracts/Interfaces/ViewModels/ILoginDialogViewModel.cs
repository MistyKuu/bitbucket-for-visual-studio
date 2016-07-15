using System.Windows.Input;

namespace BitBucketVs.Contracts.Interfaces.ViewModels
{
    public interface ILoginDialogViewModel
    {
        string Login { get; set; }
        string Password { get; set; }
        string Error { get; set; }
        ICommand ConnectCommand { get; }
    }
}