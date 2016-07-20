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
using GitClientVS.Infrastructure.Events;
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
        private readonly ReactiveCommand<object> _cloneCommand;
        private readonly ReactiveCommand<object> _choosePathCommand;
        private IEnumerable<GitRemoteRepository> _repositories;
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _errorMessage;
        private GitRemoteRepository _selectedRepository;
        private string _clonePath;

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

        [Required]
        public GitRemoteRepository SelectedRepository
        {
            get { return _selectedRepository; }
            set { this.RaiseAndSetIfChanged(ref _selectedRepository, value); }
        }

        [Required(AllowEmptyStrings = false)]
        public string ClonePath
        {
            get { return _clonePath; }
            set { this.RaiseAndSetIfChanged(ref _clonePath, value); }
        }

        public ICommand CloneCommand => _cloneCommand;
        public ICommand ChoosePathCommand => _choosePathCommand;

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
            _cloneCommand = ReactiveCommand.Create(CanExecuteCloneObservable());
            _choosePathCommand = ReactiveCommand.Create(Observable.Return(true));

            ClonePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), "Source", "Repos");

            SetupObservables();
        }

        private void SetupObservables()
        {
            _cloneCommand.Subscribe(_ => Clone());
            _cloneCommand.Subscribe(_ => OnClose());
            _cloneCommand.ThrownExceptions.Subscribe(OnError);
            _choosePathCommand.Subscribe(_ => ChooseClonePath());
        }

        private void ChooseClonePath()
        {
            var result = _fileService.OpenDirectoryDialog(ClonePath);
            if (result.IsSuccess)
                ClonePath = result.Data;
        }

        public async Task InitializeAsync()
        {
            await RefreshRepositories();
        }

        private void OnError(Exception ex)
        {
            ErrorMessage = ex.Message;
        }

        private void Clone()
        {
            _gitService.CloneRepository(SelectedRepository.CloneUrl, SelectedRepository.Name, ClonePath);
        }


        private async Task RefreshRepositories()
        {
            Repositories = await _gitClientService.GetUserRepositoriesAsync();
            SelectedRepository = Repositories.FirstOrDefault();
        }

        private IObservable<bool> CanExecuteCloneObservable()
        {
            return ValidationObservable.Select(x => CanExecute()).StartWith(CanExecute());
        }

        private bool CanExecute()
        {
            return IsObjectValid();
        }



        protected void OnClose()
        {
            Closed?.Invoke(this, new EventArgs());
        }

        public event EventHandler Closed;
    }
}
