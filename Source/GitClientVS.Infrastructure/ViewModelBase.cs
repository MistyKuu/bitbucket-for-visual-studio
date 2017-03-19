using System;
using System.Collections.Generic;
using GitClientVS.Contracts;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Infrastructure.Extensions;
using ReactiveUI;

namespace GitClientVS.Infrastructure
{
    public abstract class ViewModelBase : ReactiveValidatedObject, IViewModel
    {
        protected ViewModelBase()
        {
            (this as IViewModelWithCommands)?.InitializeCommands();
            (this as ILoadableViewModel)?.SetupLoadingCommands();
            (this as IViewModelWithErrorMessage)?.CatchCommandErrors();
          
        }
    }
}
