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
    [Export(typeof(ICreatePullRequestsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CreatePullRequestsViewModel : ViewModelBase, ICreatePullRequestsViewModel
    {
        private readonly IGitClientService _gitClientService;
        private readonly IGitService _gitService;
        private ReactiveCommand<Unit> _initializeCommand;
        private bool _isLoading;
        private string _errorMessage;
        private ReactiveCommand<Unit> _createNewPullRequestCommand;
        private IEnumerable<GitBranch> _branches;


        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { this.RaiseAndSetIfChanged(ref _errorMessage, value); }
        }

        public IEnumerable<GitBranch> Branches
        {
            get { return _branches; }
            set { this.RaiseAndSetIfChanged(ref _branches, value); }
        }

        public IEnumerable<IReactiveCommand> ThrowableCommands => new[] { _initializeCommand, _createNewPullRequestCommand };
        public IEnumerable<IReactiveCommand> LoadingCommands => new[] { _initializeCommand, _createNewPullRequestCommand };

        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }

        public ICommand InitializeCommand => _initializeCommand;
        public ICommand CreateNewPullRequestCommand => _createNewPullRequestCommand;

        [ImportingConstructor]
        public CreatePullRequestsViewModel(IGitClientService gitClientService, IGitService gitService)
        {
            _gitClientService = gitClientService;
            _gitService = gitService;
        }

        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateAsyncTask(CanLoadPullRequests(), _ => LoadBranches());
            _createNewPullRequestCommand = ReactiveCommand.CreateAsyncTask(Observable.Return(true), _ => CreateNewPullRequest());
        }

        private async Task CreateNewPullRequest()
        {
            var currentRepo = (_gitService.GetActiveRepository()).Name;
            var gitPullRequest = new GitPullRequest("asd", "asd", "asd", "asd");
            await _gitClientService.CreatePullRequest(gitPullRequest, currentRepo);
        }

        private async Task LoadBranches()
        {
            Branches = await _gitClientService.GetBranches(_gitService.GetActiveRepository().Name);
        }

        private IObservable<bool> CanLoadPullRequests()
        {
            return Observable.Return(true);
        }
    }
}
