using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Infrastructure.Utils;
using log4net;
using ReactiveUI;

namespace GitClientVS.Infrastructure.Extensions
{
    public static class ViewModelsExtensions
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void CatchCommandErrors(this IViewModelWithErrorMessage vm)
        {
            foreach (var reactiveCommand in vm.ThrowableCommands)
            {
                reactiveCommand.IsExecuting.Where(x => x).Subscribe(_ =>
                {
                    vm.ErrorMessage = null;
                });
                reactiveCommand.ThrownExceptions.Subscribe((ex) =>
                {
                    Logger.Error(ex);
                    vm.ErrorMessage = ExceptionMapper.Map(ex);
                });
            }

            var reactiveValidated = vm as ReactiveValidatedObject; // todo don't show at the beginning, play with initializable

            reactiveValidated?.ValidationObservable.Subscribe(_ =>
            {
                vm.ErrorMessage = reactiveValidated.Errors.FirstOrDefault();
            });

            var c = vm as ReactiveObject;
            c?.ThrownExceptions.Subscribe(ex => Logger.Error("Unexpected error: " + ex));
        }

        public static void SetupLoadingCommands(this ILoadableViewModel vm)
        {
            vm.LoadingCommands.Select(x => x.IsExecuting).Merge().Subscribe(_ =>
            {
                vm.IsLoading = vm.LoadingCommands.Any(x => x.IsExecuting.FirstAsync().Wait());
            });
        }

        public static void Initialize(this IViewModel viewModel, object parameter = null)
        {
            (viewModel as ILoadableViewModel)?.SetupLoadingCommands();
            (viewModel as IViewModelWithErrorMessage)?.CatchCommandErrors();
            (viewModel as ViewModelBase)?.InitializeObservables();
            (viewModel as IInitializable)?.InitializeCommand.Execute(parameter);
        }
    }
}
