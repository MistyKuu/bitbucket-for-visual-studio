using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using ReactiveUI;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(IPullRequestsMainViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PullRequestsMainViewModel : ViewModelBase, IPullRequestsMainViewModel
    {
        private readonly IGitClientService _gitClientService;
        private readonly IGitService _gitService;
        private ReactiveCommand<Unit> _initializeCommand;
        private bool _isLoading;
        private IEnumerable<GitPullRequest> _gitPullRequests;
        private string _errorMessage;

        public IEnumerable<GitPullRequest> GitPullRequests
        {
            get { return _gitPullRequests; }
            set { this.RaiseAndSetIfChanged(ref _gitPullRequests, value); }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { this.RaiseAndSetIfChanged(ref _errorMessage, value); }
        }

        public IEnumerable<IReactiveCommand> ThrowableCommands => new[] { _initializeCommand };
        public IEnumerable<IReactiveCommand> LoadingCommands => new[] { _initializeCommand };

        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }

        public ICommand InitializeCommand => _initializeCommand;

        [ImportingConstructor]
        public PullRequestsMainViewModel(IGitClientService gitClientService, IGitService gitService)
        {
            _gitClientService = gitClientService;
            _gitService = gitService;
        }

        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateAsyncTask(CanLoadPullRequests(), _ => LoadPullRequests());
        }

        private async Task LoadPullRequests()
        {
            GitPullRequests = await _gitClientService.GetPullRequests("django-piston", "jespern");
        }

        private IObservable<bool> CanLoadPullRequests()
        {
            return Observable.Return(true);
        }


    }
}
