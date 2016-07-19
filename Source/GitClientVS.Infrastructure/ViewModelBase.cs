using GitClientVS.Contracts;
using GitClientVS.Contracts.Interfaces;
using ReactiveUI;

namespace GitClientVS.Infrastructure
{
    public abstract class ViewModelBase : ReactiveValidatedObject, IViewModel
    {
    }
}
