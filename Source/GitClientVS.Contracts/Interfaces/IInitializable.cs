using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

namespace GitClientVS.Contracts.Interfaces
{
    public interface IInitializable
    {
        ICommand InitializeCommand { get; }
    }
}
