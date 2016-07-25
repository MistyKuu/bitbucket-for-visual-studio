using System.Collections.Generic;
using ReactiveUI;

namespace GitClientVS.Contracts.Interfaces
{
    public interface ILoadableViewModel
    {
        bool IsLoading { get; set; }
        IEnumerable<IReactiveCommand> LoadingCommands { get; }
    }
}
