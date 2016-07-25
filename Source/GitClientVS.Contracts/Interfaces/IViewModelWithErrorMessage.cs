using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

namespace GitClientVS.Contracts.Interfaces
{
    public interface IViewModelWithErrorMessage : IViewModelWithCommands
    {
        string ErrorMessage { get; set; }
        IEnumerable<IReactiveCommand> ThrowableCommands { get; }
    }
}
