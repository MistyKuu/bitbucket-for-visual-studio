using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Infrastructure.ViewModels;
using GitClientVS.VisualStudio.UI.Window;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ReactiveUI;

namespace GitClientVS.VisualStudio.UI.Services
{
    [Export(typeof(ICommandsService))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CommandsService : ICommandsService
    {
        private readonly ExportFactory<IDiffWindowControlViewModel> _diffFactory;
        private readonly ExportFactory<IPullRequestsWindowContainerViewModel> _pqFactory;
        private readonly IPageNavigationService<IPullRequestsWindow> _navigationService;
        private readonly IGitClientServiceFactory _gitClientServiceFactory;
        private Package _package;

        [ImportingConstructor]
        public CommandsService(
            ExportFactory<IDiffWindowControlViewModel> diffFactory,
            ExportFactory<IPullRequestsWindowContainerViewModel> pqFactory,
            IPageNavigationService<IPullRequestsWindow> navigationService,
            IGitClientServiceFactory gitClientServiceFactory
            )
        {
            _diffFactory = diffFactory;
            _pqFactory = pqFactory;
            _navigationService = navigationService;
            _gitClientServiceFactory = gitClientServiceFactory;
        }

        public void Initialize(object package)
        {
            _package = (Package)package;
        }


        public void ShowPullRequestsWindow()
        {
            var gitClientService = _gitClientServiceFactory.GetService();
            var window = ShowWindow<PullRequestsWindow>();
            var vm = _pqFactory.CreateExport().Value;
            var view = window.Content as IPullRequestsWindowContainer;
            window.Caption = $"{gitClientService.Origin} - Pull Requests";
            view.DataContext = vm;
            view.Window = window;
            _navigationService.ClearNavigationHistory();
            _navigationService.Navigate<IPullRequestsMainView>();
        }

        public void ShowDiffWindow(object parameter, int id)
        {
            var window = ShowWindow<DiffWindow>(id);
            var vm = (DiffWindowControlViewModel)_diffFactory.CreateExport().Value;
            var view = window.Content as IView;
            view.DataContext = vm;
            vm.InitializeCommand.Execute(parameter);
            vm.WhenAnyValue(x => x.FileDiff).Where(x => x != null).Subscribe(x => window.Caption = $"Diff ({x.From})");
        }

        private TWindow ShowWindow<TWindow>(int id = 0) where TWindow : class
        {
            if (_package == null)
                throw new Exception("Package wasn't initialized");

            ToolWindowPane window = _package.FindToolWindow(typeof(TWindow), id, true);

            if (window?.Frame == null)
                throw new NotSupportedException("Cannot create window");

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

            return window as TWindow;
        }
    }
}
