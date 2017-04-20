using System;
using System.Collections.Generic;
using GitClientVS.Contracts;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Infrastructure.Extensions;
using ReactiveUI;
using System.Linq;

namespace GitClientVS.Infrastructure
{
    public abstract class ViewModelBase : ReactiveValidatedObject, IViewModel, IDisposable
    {
        private IEnumerable<IDisposable> _disposables;

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
            foreach (var obs in _disposables)
                obs?.Dispose();
        }
    }
}
