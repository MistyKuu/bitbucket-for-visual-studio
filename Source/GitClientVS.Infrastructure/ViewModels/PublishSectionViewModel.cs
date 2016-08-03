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
        private ReactiveCommand<Unit> _publishRepositoryCommand;
        private string _repositoryName;
        private string _description;
        private bool _isPrivate;
        private string _errorMessage;
        private bool _isLoading;
        public ICommand PublishRepositoryCommand => _publishRepositoryCommand;

        [ImportingConstructor]
        public PublishSectionViewModel(
            IGitClientService gitClientService,
            IGitService gitService,
            IFileService fileService
            )
        {
            _gitClientService = gitClientService;
            _gitService = gitService;
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

        //public object Owners
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //public object Owner
        //{
        //    get { throw new NotImplementedException(); }
        //}


        public void InitializeCommands()
        {
            _publishRepositoryCommand = ReactiveCommand.CreateAsyncTask(CanPublishRepository(), _ => PublishRepository());
        }

        private async Task PublishRepository()
        {
            var gitRemoteRepository = new GitRemoteRepository()
            {
                Name = RepositoryName,
                Description = Description,
                IsPrivate = IsPrivate
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

        public IEnumerable<IReactiveCommand> ThrowableCommands => new[] { _publishRepositoryCommand };
        public IEnumerable<IReactiveCommand> LoadingCommands => new[] { _publishRepositoryCommand };


    }
}
