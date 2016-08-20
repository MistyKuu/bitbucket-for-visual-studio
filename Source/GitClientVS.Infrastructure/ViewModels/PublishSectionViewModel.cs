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
using GitClientVS.Contracts.Events;
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
    [Export(typeof(IPublishSectionViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PublishSectionViewModel : ViewModelBase, IPublishSectionViewModel
    {
        private IGitClientService _gitClientService;
        private IGitService _gitService;
        private readonly IUserInformationService _userInformationService;
        private readonly IEventAggregatorService _eventAggregator;
        private readonly IGitWatcher _gitWatcher;
        private ReactiveCommand<Unit> _publishRepositoryCommand;
        private ReactiveCommand<Unit> _initializeCommand;
        private string _repositoryName;
        private string _description;
        private bool _isPrivate;
        private string _errorMessage;
        private bool _isLoading;
        private List<string> _owners;
        private string _selectedOwner;

        public ICommand PublishRepositoryCommand => _publishRepositoryCommand;
        public ICommand InitializeCommand => _initializeCommand;


        [ImportingConstructor]
        public PublishSectionViewModel(
            IGitClientService gitClientService,
            IGitService gitService,
            IFileService fileService,
            IUserInformationService userInformationService,
            IEventAggregatorService eventAggregator,
            IGitWatcher gitWatcher
            )
        {
            _gitClientService = gitClientService;
            _gitService = gitService;
            _userInformationService = userInformationService;
            _eventAggregator = eventAggregator;
            _gitWatcher = gitWatcher;
        }


        [Required]
        public string RepositoryName
        {
            get { return _repositoryName; }
            set { this.RaiseAndSetIfChanged(ref _repositoryName, value); }
        }

        [Required]
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

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                this.RaiseAndSetIfChanged(ref _errorMessage, value);
            }
        }
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                this.RaiseAndSetIfChanged(ref _isLoading, value);
            }
        }

        public List<string> Owners
        {
            get { return _owners; }
            set
            {
                this.RaiseAndSetIfChanged(ref _owners, value);
            }
        }

        [Required]
        public string SelectedOwner
        {
            get { return _selectedOwner; }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedOwner, value);
            }
        }


        public void InitializeCommands()
        {
            _publishRepositoryCommand = ReactiveCommand.CreateAsyncTask(CanPublishRepository(), _ => PublishRepository());
            _initializeCommand = ReactiveCommand.CreateAsyncTask(Observable.Return(true), _ => CreateOwners());
            _publishRepositoryCommand.Subscribe(_ => _gitWatcher.Refresh());
        }

        private async Task CreateOwners()
        {
            var teamNames = (await _gitClientService.GetTeams()).Select(x => x.Name).ToList();
            teamNames.Insert(0, _userInformationService.ConnectionData.UserName);
            Owners = teamNames;
            SelectedOwner = Owners.FirstOrDefault();
        }

        private async Task PublishRepository()
        {
            var gitRemoteRepository = new GitRemoteRepository()
            {
                Name = RepositoryName,
                Description = Description,
                IsPrivate = IsPrivate,
                Owner = SelectedOwner
            };

            var remoteRepo = await _gitClientService.CreateRepositoryAsync(gitRemoteRepository);
            _gitService.PublishRepository(remoteRepo);
        }

        private IObservable<bool> CanPublishRepository()
        {
            return ValidationObservable.Select(x => CanExecute()).StartWith(CanExecute());
        }

        private bool CanExecute()
        {
            return IsObjectValid();
        }

        public IEnumerable<IReactiveCommand> ThrowableCommands => new[] { _publishRepositoryCommand, _initializeCommand };
        public IEnumerable<IReactiveCommand> LoadingCommands => new[] { _publishRepositoryCommand, _initializeCommand };


    }
}
