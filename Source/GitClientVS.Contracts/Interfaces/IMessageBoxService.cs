using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GitClientVS.Contracts.Interfaces
{
    public interface IMessageBoxService
    {
        void ExecuteCommandWithConfirmation(string title, string message, ICommand command);
    }
}
