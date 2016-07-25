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
    [Export(typeof(ICreateRepositoriesDialogViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CreateRepositoriesDialogViewModel : ViewModelBase, ICreateRepositoriesDialogViewModel
    {
        private readonly IGitClientService _gitClientService;
        private readonly IGitService _gitService;
        private readonly IFileService _fileService;
        private ReactiveCommand<Unit> _createCommand;
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string _errorMessage;
        private GitRemoteRepository _repository;
        private string _localPath;
        private string _name;
        private string _description;
        private bool _isPrivate;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { this.RaiseAndSetIfChanged(ref _errorMessage, value); }
        }

     

        [Required]
        public string LocalPath
        {
            get { return _localPath; }
            set { this.RaiseAndSetIfChanged(ref _localPath, value); }
        }

        [Required]
        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }

        public string Description
        {
            get { return _description; }
            set { this.RaiseAndSetIfChanged(ref _description, value); }
        }

        public bool IsPrivate
        {
            get { return _isPrivate; }
            set { this.RaiseAndSetIfChanged(ref _isPrivate, value); }
        }


        public ICommand CreateCommand => _createCommand;
        public IEnumerable<IReactiveCommand> ThrowableCommands => new List<IReactiveCommand> { _createCommand };

        [ImportingConstructor]
        public CreateRepositoriesDialogViewModel(
            IGitClientService gitClientService,
            IGitService gitService,
            IFileService fileService
            )
        {
            _gitClientService = gitClientService;
            _gitService = gitService;
            _fileService = fileService;
            LocalPath = Paths.DefaultRepositoryPath;

            SetupObservables();
        }

        public void InitializeCommands()
        {
            _createCommand = ReactiveCommand.CreateAsyncTask(CanExecuteCreateObservable(), _ => Create());
        }

        private void SetupObservables()
        {
            _createCommand.Subscribe(_ => OnClose());
        }


        private async Task Create()
        {
            var repository = new GitRemoteRepository { Name = Name, Description = Description, IsPrivate = IsPrivate };
            var remoteRepo = await _gitClientService.CreateRepositoryAsync(repository);
            _gitService.CloneRepository(remoteRepo.CloneUrl, remoteRepo.Name, LocalPath);
        }


        private IObservable<bool> CanExecuteCreateObservable()
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
