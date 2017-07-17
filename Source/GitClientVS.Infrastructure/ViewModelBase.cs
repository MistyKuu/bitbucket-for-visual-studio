using System;
using System.Collections.Generic;
using GitClientVS.Contracts;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Infrastructure.Extensions;
using ReactiveUI;
using System.Linq;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Contracts.Models.Tree;
using GitClientVS.Infrastructure.ViewModels;
using ParseDiff;

namespace GitClientVS.Infrastructure
{
    public abstract class ViewModelBase : ReactiveValidatedObject, IViewModel, IDisposable
    {
        private IEnumerable<IDisposable> _disposables;

        protected ViewModelBase()
        {
            (this as IViewModelWithCommands)?.InitializeCommands();
        }

        public void InitializeObservables()
        {
            _disposables = SetupObservables().ToList();
        }

        protected virtual IEnumerable<IDisposable> SetupObservables()
        {
            return Enumerable.Empty<IDisposable>();
        }

        public virtual void Dispose()
        {
            foreach (var obs in _disposables ?? Enumerable.Empty<IDisposable>())
                obs?.Dispose();
        }
    }
}
