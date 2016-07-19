using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Diagnostics;
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
    [Export(typeof(ICloneRepositoriesViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CloneRepositoriesViewModel : ViewModelBase, ICloneRepositoriesViewModel
    {
        private readonly IGitClientService _gitClientService;
        private readonly IGitService _gitService;
        private readonly ReactiveCommand<object> _cloneCommand;
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


        [ImportingConstructor]
        public CloneRepositoriesViewModel(
            IEventAggregatorService eventAggregator,
            IGitClientService gitClientService,
            IGitService gitService
            )
        {
            _gitClientService = gitClientService;
            _gitService = gitService;
            _cloneCommand = ReactiveCommand.Create(CanExecuteObservable());
            _cloneCommand.Subscribe(_ => Clone());
            _cloneCommand.Subscribe(_ => OnClose());
            _cloneCommand.ThrownExceptions.Subscribe(OnError);
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
            _gitService.CloneRepository(SelectedRepository.Links.Clone.First().Href, SelectedRepository.Name, ClonePath);
        }


        private async Task RefreshRepositories()
        {
            Repositories = await _gitClientService.GetUserRepositoriesAsync();
            SelectedRepository = Repositories.FirstOrDefault();
        }

        private IObservable<bool> CanExecuteObservable()
        {
            return ValidationObservable.Select(x => CanExecute()).StartWith(CanExecute());
        }

        private bool CanExecute()
        {
            return IsObjectValid();
        }

        public ICommand CloneCommand => _cloneCommand;


        protected void OnClose()
        {
            Closed?.Invoke(this, new EventArgs());
        }

        public event EventHandler Closed;
    }
}
