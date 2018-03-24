using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive.Linq;
using System.Windows.Input;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Infrastructure.ViewModels
{
    [Export(typeof(IPublishSectionViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PublishSectionViewModel : ViewModelBase, IPublishSectionViewModel
    {
        private IGitClientService _gitClientService;
        private IGitService _gitService;
        private readonly IUserInformationService _userInformationService;
        private readonly IGitWatcher _gitWatcher;
        private ReactiveCommand _publishRepositoryCommand;
        private ReactiveCommand _initializeCommand;
        private string _repositoryName;
        private string _description;
        private bool _isPrivate;
        private string _errorMessage;
        private bool _isLoading;
        private List<Owner> _owners;
        private Owner _selectedOwner;

        public ICommand PublishRepositoryCommand => _publishRepositoryCommand;
        public ICommand InitializeCommand => _initializeCommand;

        [Required]
        public string RepositoryName
        {
            get => _repositoryName;
            set => this.RaiseAndSetIfChanged(ref _repositoryName, value);
        }

        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        public bool IsPrivate
        {
            get => _isPrivate;
            set => this.RaiseAndSetIfChanged(ref _isPrivate, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
        }
        public bool IsLoading
        {
            get => _isLoading;
            set => this.RaiseAndSetIfChanged(ref _isLoading, value);
        }

        public List<Owner> Owners
        {
            get => _owners;
            set => this.RaiseAndSetIfChanged(ref _owners, value);
        }

        [Required]
        public Owner SelectedOwner
        {
            get => _selectedOwner;
            set => this.RaiseAndSetIfChanged(ref _selectedOwner, value);
        }

        public string GitClientType => _gitClientService.GitClientType;

        public IEnumerable<ReactiveCommand> ThrowableCommands => new[] { _publishRepositoryCommand, _initializeCommand };
        public IEnumerable<ReactiveCommand> LoadingCommands => new[] { _publishRepositoryCommand, _initializeCommand };


        [ImportingConstructor]
        public PublishSectionViewModel(
            IGitClientService gitClientService,
            IGitService gitService,
            IFileService fileService,
            IUserInformationService userInformationService,
            IGitWatcher gitWatcher
            )
        {
            _gitClientService = gitClientService;
            _gitService = gitService;
            _userInformationService = userInformationService;
            _gitWatcher = gitWatcher;
            _isPrivate = true;
        }



        public void InitializeCommands()
        {
            _publishRepositoryCommand = ReactiveCommand.CreateFromTask(_ => PublishRepository(), CanPublishRepository());
            _initializeCommand = ReactiveCommand.CreateFromTask(_ => CreateOwners());
        }

        private async Task CreateOwners()
        {
            if (!_userInformationService.ConnectionData.IsLoggedIn)
                return;

            var teamNames = (await _gitClientService.GetTeams()).Select(x => new Owner() { Name = x.Name, IsTeam = true }).ToList();
            teamNames.Insert(0, new Owner() { Name = _userInformationService.ConnectionData.UserName, IsTeam = false });
            Owners = teamNames;
            SelectedOwner = Owners.FirstOrDefault();
        }

        private async Task PublishRepository()
        {
            var gitRemoteRepository = new GitRemoteRepository()
            {
                Name = RepositoryName.Replace(' ', '-'),
                Description = Description,
                IsPrivate = IsPrivate,
                Owner = SelectedOwner.Name,
                IsTeam = SelectedOwner.IsTeam
            };

            var remoteRepo = await _gitClientService.CreateRepositoryAsync(gitRemoteRepository);
            _gitService.PublishRepository(remoteRepo);
            _gitWatcher.Refresh();
        }

        private IObservable<bool> CanPublishRepository()
        {
            return ValidationObservable.Select(x => Unit.Default)
                .Merge(Changed.Select(x => Unit.Default))
                .Select(x => CanExecute()).StartWith(CanExecute());
        }

        private bool CanExecute()
        {
            return IsObjectValid();
        }


    }


    public class Owner
    {
        public string Name { get; set; }
        public bool IsTeam { get; set; }

        public override string ToString()
        {
            return $"{Name} {(IsTeam ? " (Team)" : string.Empty)}";
        }
    }
}
