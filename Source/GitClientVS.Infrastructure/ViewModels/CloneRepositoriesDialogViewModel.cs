using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces;
using ReactiveUI;
using System.Reactive.Linq;
using System.Security;
using System.Windows.Input;
using BitBucket.REST.API;
using BitBucket.REST.API.Models;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using log4net;
using log4net.Config;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(ICloneRepositoriesDialogViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CloneRepositoriesDialogViewModel : ViewModelBase, ICloneRepositoriesDialogViewModel
    {
        private readonly IGitClientService _gitClientService;
        private readonly IGitService _gitService;
        private readonly IFileService _fileService;
        private readonly IUserInformationService _userInformationService;
        private ReactiveCommand<Unit> _cloneCommand;
        private ReactiveCommand<object> _choosePathCommand;
        private ReactiveCommand<Unit> _initializeCommand;
        private IEnumerable<GitRemoteRepository> _repositories;
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _errorMessage;
        private GitRemoteRepository _selectedRepository;
        private string _clonePath;
        private bool _isLoading;
        private string _filterRepoName;
        private ReactiveList<GitRemoteRepository> _filteredRepositories;


        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { this.RaiseAndSetIfChanged(ref _errorMessage, value); }
        }

        public IEnumerable<GitRemoteRepository> Repositories
        {
            get { return _repositories; }
            set { this.RaiseAndSetIfChanged(ref _repositories, value); }
        }

        public GitRemoteRepository SelectedRepository
        {
            get { return _selectedRepository; }
            set { this.RaiseAndSetIfChanged(ref _selectedRepository, value); }
        }

        [Required(AllowEmptyStrings = false)]
        [ValidatesViaMethod(AllowBlanks = false, AllowNull = false, Name = nameof(ClonePathNotExists), ErrorMessage = "Directory already exists")]
        [ValidatesViaMethod(AllowBlanks = false, AllowNull = false, Name = nameof(ClonePathHasSelectedRepository), ErrorMessage = "Please select repository")]
        [ValidatesViaMethod(AllowBlanks = false, AllowNull = false, Name = nameof(ClonePathIsPath), ErrorMessage = "Clone Path must be a valid path")]
        public string ClonePath
        {
            get { return _clonePath; }
            set { this.RaiseAndSetIfChanged(ref _clonePath, value); }
        }



        public ICommand CloneCommand => _cloneCommand;
        public ICommand ChoosePathCommand => _choosePathCommand;
        public ICommand InitializeCommand => _initializeCommand;

        public IEnumerable<IReactiveCommand> ThrowableCommands => new[] { _cloneCommand, _initializeCommand };
        public IEnumerable<IReactiveCommand> LoadingCommands => new[] { _cloneCommand, _initializeCommand };

        [ImportingConstructor]
        public CloneRepositoriesDialogViewModel(
            IGitClientService gitClientService,
            IGitService gitService,
            IFileService fileService,
            IUserInformationService userInformationService
            )
        {
            _gitClientService = gitClientService;
            _gitService = gitService;
            _fileService = fileService;
            _userInformationService = userInformationService;

            var gitClonePath = _gitService.GetDefaultRepoPath();
            ClonePath = !string.IsNullOrEmpty(gitClonePath) ? gitClonePath : Paths.DefaultRepositoryPath;

            SetupObservables();
        }

        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateAsyncTask(CanRefreshObservable(), _ => RefreshRepositories());
            _cloneCommand = ReactiveCommand.CreateAsyncTask(CanExecuteCloneObservable(), _ => Clone());
            _choosePathCommand = ReactiveCommand.Create(Observable.Return(true));
        }

        private void SetupObservables()
        {
            _cloneCommand.Subscribe(_ => OnClose());
            _choosePathCommand.Subscribe(_ => ChooseClonePath());
            this.WhenAnyValue(x => x.SelectedRepository).Subscribe(_ => ForcePropertyValidation(nameof(ClonePath)));
            this.WhenAnyValue(x => x.FilterRepoName, x => x.Repositories)
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Where(x => x.Item2 != null && x.Item2.Any())
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(_ => Filter().OrderBy(x => x.Name))
                .Subscribe(filteredRepos =>
                {
                    FilteredRepositories = new ReactiveList<GitRemoteRepository>(filteredRepos);
                    SelectedRepository = FilteredRepositories.FirstOrDefault();
                });

        }

        public string FilterRepoName
        {
            get { return _filterRepoName; }
            set
            {
                this.RaiseAndSetIfChanged(ref _filterRepoName, value);
            }
        }

        public ReactiveList<GitRemoteRepository> FilteredRepositories
        {
            get { return _filteredRepositories; }
            set { this.RaiseAndSetIfChanged(ref _filteredRepositories, value); }
        }

        private void ChooseClonePath()
        {
            var result = _fileService.OpenDirectoryDialog(ClonePath);
            if (result.IsSuccess)
                ClonePath = result.Data;
        }

        private async Task Clone()
        {
            await Task.Run(() =>
            {
                _gitService.CloneRepository(SelectedRepository.CloneUrl, SelectedRepository.Name, ClonePath);
            });
        }

        private IObservable<bool> CanRefreshObservable()
        {
            return this.WhenAnyValue(x => x.IsLoading).Select(loading => !loading);
        }

        private async Task RefreshRepositories()
        {
            Repositories = await _gitClientService.GetAllRepositories();
        }

        private IObservable<bool> CanExecuteCloneObservable()
        {
            var obs = this.WhenAnyValue(x => x.ClonePath, x => x.SelectedRepository).Select(x => CanExecute());
            var valObs = ValidationObservable.Select(x => CanExecute()).StartWith(CanExecute());
            return obs.Merge(valObs);
        }

        private bool CanExecute()
        {
            return IsObjectValid();
        }



        public bool ClonePathHasSelectedRepository(string clonePath)
        {
            return SelectedRepository != null;
        }

        public bool ClonePathNotExists(string clonePath)
        {
            if (SelectedRepository == null)
                return false;

            return !Directory.Exists(Path.Combine(ClonePath, SelectedRepository.Name));
        }

        public bool ClonePathIsPath(string clonePath)
        {
            return Path.IsPathRooted(clonePath) && clonePath.IndexOfAny(Path.GetInvalidPathChars()) == -1;
        }

        private IEnumerable<GitRemoteRepository> Filter()
        {
            return string.IsNullOrEmpty(FilterRepoName) ?
                Repositories :
                Repositories.Where(repo => repo.Name.Contains(FilterRepoName, StringComparison.InvariantCultureIgnoreCase));
        }

        protected void OnClose()
        {
            Closed?.Invoke(this, new EventArgs());
        }

        public event EventHandler Closed;

    }
}
