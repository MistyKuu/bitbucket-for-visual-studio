using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces;

namespace GitClientVS.Infrastructure.Extensions
{
    public static class ViewModelsExtensions
    {
        public static void CatchCommandErrors(this IViewModelWithErrorMessage vm)
        {
            foreach (var reactiveCommand in vm.CatchableCommands)
                reactiveCommand.ThrownExceptions.Subscribe((ex) => vm.ErrorMessage = ex.Message);
        }

        public static void SetupLoadingCommands(this ILoadableViewModel vm)
        {
            var res = Observable.Empty<bool>();
            foreach (var cmd in vm.LoadingCommands)
                res = cmd.IsExecuting.Merge(res);

            res.Subscribe(x =>
            {
                vm.IsLoading = x;
            });
        }
    }
}
