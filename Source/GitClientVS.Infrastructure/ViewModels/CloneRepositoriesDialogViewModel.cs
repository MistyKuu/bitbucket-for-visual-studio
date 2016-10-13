using System;
using System.Collections.Generic;
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
        private ReactiveCommand<Unit> _cloneCommand;
        private ReactiveCommand<object> _choosePathCommand;
        private ReactiveCommand<Unit> _initializeCommand;
        private IEnumerable<GitRepository> _repositories;
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _errorMessage;
        private GitRepository _selectedRepository;
        private string _clonePath;
        private bool _isLoading;


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

        public IEnumerable<GitRepository> Repositories
        {
            get { return _repositories; }
            set { this.RaiseAndSetIfChanged(ref _repositories, value); }
        }

        public GitRepository SelectedRepository
        {
            get { return _selectedRepository; }
            set { this.RaiseAndSetIfChanged(ref _selectedRepository, value); }
        }

        [Required(AllowEmptyStrings = false)]
        [ValidatesViaMethod(AllowBlanks = false, AllowNull = false, Name = nameof(ClonePathNotExists), ErrorMessage = "Directory already exists")]
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
            IGitClientServiceFactory gitClientFactory,
            IGitService gitService,
            IFileService fileService
            )
        {
            _gitClientService = gitClientFactory.GetService();
            _gitService = gitService;
            _fileService = fileService;
            ClonePath = Paths.DefaultRepositoryPath;
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
            this.WhenAnyValue(x => x.SelectedRepository).Subscribe(_ =>
            {
                var clonePath = ClonePath;//TODO CHANGE IT LATER, REVALIDATE CLONEPATH WHEN SELECTEDREPOSITORYCHANGED
                ClonePath = null;
                ClonePath = clonePath;
            });
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
            SelectedRepository = Repositories.FirstOrDefault();
        }

        private IObservable<bool> CanExecuteCloneObservable()
        {
            var obs = this.WhenAnyValue(x => x.ClonePath, x => x.SelectedRepository.Name).Select(x => CanExecute());
            var valObs = ValidationObservable.Select(x => CanExecute()).StartWith(CanExecute());
            return obs.Merge(valObs);
        }

        private bool CanExecute()
        {
            return IsObjectValid();
        }

        public bool ClonePathNotExists(string clonePath)
        {
            if (SelectedRepository == null)
                return false;

            return !Directory.Exists(Path.Combine(ClonePath, SelectedRepository.Name));
        }

        public bool ClonePathIsPath(string clonePath)
        {
            return  Path.IsPathRooted(clonePath) && clonePath.IndexOfAny(Path.GetInvalidPathChars()) == -1;
        }


        protected void OnClose()
        {
            Closed?.Invoke(this, new EventArgs());
        }

        public event EventHandler Closed;

    }
}
