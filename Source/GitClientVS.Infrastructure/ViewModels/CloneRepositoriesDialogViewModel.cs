using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive.Linq;
using System.Windows.Input;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(ICloneRepositoriesDialogViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CloneRepositoriesDialogViewModel : ViewModelBase, ICloneRepositoriesDialogViewModel
    {
        private readonly IGitClientService _gitClientService;
        private readonly IGitService _gitService;
        private readonly IFileService _fileService;
        private ReactiveCommand _cloneCommand;
        private ReactiveCommand _choosePathCommand;
        private ReactiveCommand<Unit, Unit> _initializeCommand;
        private IEnumerable<GitRemoteRepository> _repositories;
        private string _errorMessage;
        private GitRemoteRepository _selectedRepository;
        private string _clonePath;
        private bool _isLoading;
        private string _filterRepoName;
        private ReactiveList<GitRemoteRepository> _filteredRepositories;


        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }

        public IEnumerable<GitRemoteRepository> Repositories
        {
            get => _repositories;
            set => this.RaiseAndSetIfChanged(ref _repositories, value);
        }

        public GitRemoteRepository SelectedRepository
        {
            get => _selectedRepository;
            set => this.RaiseAndSetIfChanged(ref _selectedRepository, value);
        }

        [Required(AllowEmptyStrings = false)]
        [ValidatesViaMethod(AllowBlanks = false, AllowNull = false, Name = nameof(ClonePathNotExists), ErrorMessage = "Directory already exists")]
        [ValidatesViaMethod(AllowBlanks = false, AllowNull = false, Name = nameof(ClonePathHasSelectedRepository), ErrorMessage = "Please select repository")]
        [ValidatesViaMethod(AllowBlanks = false, AllowNull = false, Name = nameof(ClonePathIsPath), ErrorMessage = "Clone Path must be a valid path")]
        public string ClonePath
        {
            get => _clonePath;
            set => this.RaiseAndSetIfChanged(ref _clonePath, value);
        }

        public string FilterRepoName
        {
            get => _filterRepoName;
            set => this.RaiseAndSetIfChanged(ref _filterRepoName, value);
        }

        public ReactiveList<GitRemoteRepository> FilteredRepositories
        {
            get => _filteredRepositories;
            set => this.RaiseAndSetIfChanged(ref _filteredRepositories, value);
        }

        public ICommand CloneCommand => _cloneCommand;
        public ICommand ChoosePathCommand => _choosePathCommand;
        public ICommand InitializeCommand => _initializeCommand;

        public IEnumerable<ReactiveCommand> ThrowableCommands => new[] { _cloneCommand, _initializeCommand, _choosePathCommand };
        public IEnumerable<ReactiveCommand> LoadingCommands => new[] { _cloneCommand, _initializeCommand,_choosePathCommand };

        [ImportingConstructor]
        public CloneRepositoriesDialogViewModel(
            IGitClientService gitClientService,
            IGitService gitService,
            IFileService fileService
            )
        {
            _gitClientService = gitClientService;
            _gitService = gitService;
            _fileService = fileService;

            var gitClonePath = _gitService.GetDefaultRepoPath();
            ClonePath = !string.IsNullOrEmpty(gitClonePath) ? gitClonePath : Paths.DefaultRepositoryPath;
        }

        public void InitializeCommands()
        {
            _initializeCommand = ReactiveCommand.CreateFromTask(_ => RefreshRepositories(), CanRefreshObservable());
            _cloneCommand = ReactiveCommand.CreateFromTask(_ => Clone(), CanExecuteCloneObservable());
            _choosePathCommand = ReactiveCommand.CreateFromTask<object>(_ =>ChooseClonePath(),Observable.Return(true));
        }
        protected override IEnumerable<IDisposable> SetupObservables()
        {
            this.WhenAnyValue(x => x.SelectedRepository).Subscribe(_ => ForcePropertyValidation(nameof(ClonePath)));
            this.WhenAnyValue(x => x.FilterRepoName, x => x.Repositories)
                .Throttle(TimeSpan.FromMilliseconds(200), RxApp.TaskpoolScheduler)
                .Where(x => x.Item2 != null && x.Item2.Any())
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(_ => Filter().OrderBy(x => x.Name))
                .Subscribe(filteredRepos =>
                {
                    FilteredRepositories = new ReactiveList<GitRemoteRepository>(filteredRepos);
                    SelectedRepository = FilteredRepositories.FirstOrDefault();
                });

            yield break;
        }

        private Task ChooseClonePath()
        {
            var result = _fileService.OpenDirectoryDialog(ClonePath);
            if (result.IsSuccess)
                ClonePath = result.Data;

            return Task.CompletedTask;
        }

        private async Task Clone()
        {
            await Task.Run(() =>
            {
                _gitService.CloneRepository(SelectedRepository.CloneUrl, SelectedRepository.Name, ClonePath);
            });

            OnClose();
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

            return !_fileService.Exists(Path.Combine(ClonePath, SelectedRepository.Name));
        }

        public bool ClonePathIsPath(string clonePath)
        {
            return _fileService.IsPath(clonePath);
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
