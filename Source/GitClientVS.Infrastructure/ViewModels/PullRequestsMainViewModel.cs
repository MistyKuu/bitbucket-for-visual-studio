using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
        private readonly ReactiveCommand<IEnumerable<GitPullRequest>> _initializeCommand;
        private bool _isLoading;
        private IEnumerable<GitPullRequest> _gitPullRequests;
        private string _errorMessage;


        public ICommand InitializeCommand => _initializeCommand;

        public IEnumerable<GitPullRequest> GitPullRequests
        {
            get { return _gitPullRequests; }
            set
            {
                this.RaiseAndSetIfChanged(ref _gitPullRequests, value);
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { this.RaiseAndSetIfChanged(ref _errorMessage, value); }
        }

        public IEnumerable<IReactiveCommand> CatchableCommands => new List<IReactiveCommand>() { _initializeCommand };
        public IEnumerable<IReactiveCommand> LoadingCommands => new[] {  _initializeCommand };

        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }

        [ImportingConstructor]
        public PullRequestsMainViewModel(IGitClientService gitClientService)
        {
            _gitClientService = gitClientService;
            _initializeCommand = ReactiveCommand.CreateAsyncTask(CanLoadPullRequests(), _ => LoadPullRequests());
            SetupObservables();
            this.CatchCommandErrors();
            this.SetupLoadingCommands();
        }

        private void SetupObservables()
        {
            _initializeCommand.ToProperty(this, x => x.GitPullRequests);
        }

        private async Task<IEnumerable<GitPullRequest>> LoadPullRequests()
        {
            return await _gitClientService.GetPullRequests("TEST");
        }

        private IObservable<bool> CanLoadPullRequests()
        {
            return Observable.Return(true);
        }
    }
}
