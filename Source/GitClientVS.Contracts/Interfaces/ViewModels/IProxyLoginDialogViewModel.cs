using GitClientVS.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GitClientVS.Contracts.Interfaces.ViewModels
{
    public interface IProxyLoginDialogViewModel : ICloseable<bool>, IViewModelWithErrorMessage, IViewModel
    {
        string Login { get; set; }
        string Password { get; set; }
        ICommand AcceptCommand { get; }
        string ProxyUrl { get; set; }
    }
}
