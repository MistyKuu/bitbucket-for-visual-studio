using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using GitClientVS.Infrastructure.ViewModels;
using GitClientVS.Tests.Shared.Extensions;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using ReactiveUI;
using ReactiveUI.Testing;
using Rhino.Mocks;

namespace GitClientVS.Infrastructure.Tests.ViewModels
{
    [TestFixture]
    public class CloneRepositoriesDialogViewModelTests
    {
        private CloneRepositoriesDialogViewModel _sut;
        private IGitClientService _gitClientService;
        private IGitService _gitService;
        private IFileService _fileService;
        private IEventAggregatorService _eventAggregatorService;


        [SetUp]
        public void SetUp()
        {
            _gitClientService = MockRepository.GenerateMock<IGitClientService>();
            _gitService = MockRepository.GenerateMock<IGitService>();
            _fileService = MockRepository.GenerateMock<IFileService>();
            _eventAggregatorService = MockRepository.GenerateMock<IEventAggregatorService>();

            _sut = CreateSut();
        }



        [Test]
        public void OnCreate_DefaultClonePathIsCorrect_ShouldBeUsed()
        {
            _gitService.Expect(x => x.GetDefaultRepoPath()).Return("DefaultRepoPath");
            var sut = new CloneRepositoriesDialogViewModel(_gitClientService, _gitService, _fileService, _eventAggregatorService);
            Assert.That(sut.ClonePath, Is.EqualTo("DefaultRepoPath"));
        }

        [Test]
        public void OnCreate_DefaultClonePathIsInCorrect_ShouldUseDefaultRepoPath()
        {
            _gitService.Expect(x => x.GetDefaultRepoPath()).Return(string.Empty);
            var sut = new CloneRepositoriesDialogViewModel(_gitClientService, _gitService, _fileService, _eventAggregatorService);
            Assert.That(sut.ClonePath, Is.EqualTo(Paths.DefaultRepositoryPath));
        }

        [Test]
        public void Initialize_RepositoriesReturned_ShouldBeAssignedToViewModel()
        {
            IEnumerable<GitRemoteRepository> repositories = new List<GitRemoteRepository>() { new GitRemoteRepository() };
            _gitClientService.Expect(x => x.GetAllRepositories()).Return(repositories.FromTaskAsync());

            _sut.Initialize();

            Assert.That(_sut.Repositories, Is.EqualTo(repositories));
        }

        [Test]
        public void Initialize_GetRepositoriesThrowsException_ShouldSetErrorMessage()
        {
            _gitClientService.Expect(x => x.GetAllRepositories()).Throw(new Exception());
            _sut.ErrorMessage = null;

            _sut.Initialize();

            Assert.IsNull(_sut.Repositories);
            Assert.IsNotEmpty(_sut.ErrorMessage);
        }

        [Test]
        public void CloneCommand_InvalidParameters_ShouldBeDisabled()
        {
            _sut.SelectedRepository = null;

            Assert.That(_sut.CloneCommand.CanExecute(null), Is.EqualTo(false));
        }

        [Test]
        public void CloneCommand_CorrectParameters_ShouldBeEnabled()
        {
            const string path = "CorrectPath";
            const string repoName = "repoName";

            _fileService.Expect(x => x.IsPath(path)).Return(true);
            _fileService.Expect(x => x.Exists(Path.Combine(path, repoName))).Return(false);

            _sut.SelectedRepository = new GitRemoteRepository() { Name = repoName };
            _sut.ClonePath = path;

            Assert.That(_sut.CloneCommand.CanExecute(null), Is.EqualTo(true));
        }

        [Test]
        public void CloneCommand_Invoked_ShouldCloneRepository()
        {
            const string path = "CorrectPath";
            const string repoName = "repoName";

            _fileService.Expect(x => x.IsPath(path)).Return(true);
            _fileService.Expect(x => x.Exists(Path.Combine(path, repoName))).Return(false);

            _sut.SelectedRepository = new GitRemoteRepository() { Name = repoName };
            _sut.ClonePath = path;

            _gitService.Expect(x => x.CloneRepository(_sut.SelectedRepository.CloneUrl, _sut.SelectedRepository.Name, _sut.ClonePath));

            _sut.CloneCommand.Execute(null);

            while (!_sut.CloneCommand.CanExecute(null)) { } //todo temporary to satisfy tests. Fix later, can cause locks

            _gitService.VerifyAllExpectations();
        }

        [Test]
        public void CloneCommand_CloneRepositoryThrowsException_ShouldSetErrorMessage()
        {
            _gitClientService.Expect(x => x.GetAllRepositories()).Throw(new Exception());
            _sut.ErrorMessage = null;

            const string path = "CorrectPath";
            const string repoName = "repoName";

            _fileService.Expect(x => x.IsPath(path)).Return(true);
            _fileService.Expect(x => x.Exists(Path.Combine(path, repoName))).Return(false);

            _sut.SelectedRepository = new GitRemoteRepository() { Name = repoName };
            _sut.ClonePath = path;
            _sut.Initialize();

            _gitService.Expect(x => x.CloneRepository(_sut.SelectedRepository.CloneUrl, _sut.SelectedRepository.Name, _sut.ClonePath)).Throw(new Exception());

            _sut.CloneCommand.Execute(null);

            while (!_sut.CloneCommand.CanExecute(null)) { } //todo temporary to satisfy tests. Fix later, can cause locks

            _gitService.VerifyAllExpectations();
            Assert.IsNotEmpty(_sut.ErrorMessage);
        }

        [Test]
        public void CloneCommand_Invoked_ShouldRaiseCloseWindow()
        {
            const string path = "CorrectPath";
            const string repoName = "repoName";
            bool closed = false;

            _fileService.Expect(x => x.IsPath(path)).Return(true);
            _fileService.Expect(x => x.Exists(Path.Combine(path, repoName))).Return(false);

            var sut = CreateSut();

            sut.SelectedRepository = new GitRemoteRepository() { Name = repoName };
            sut.ClonePath = path;
            sut.Closed += delegate { closed = true; };

            _gitService.Expect(x => x.CloneRepository(sut.SelectedRepository.CloneUrl, sut.SelectedRepository.Name, sut.ClonePath));

            sut.Initialize();
            ((ReactiveCommand<Unit,Unit>)sut.CloneCommand).Execute(Unit.Default).Wait(); //todo temporary

            Assert.That(closed, Is.EqualTo(true));
        }


        [Test]
        public void FilterRepositories_FoundResults_ShouldFilterResults()
        {
            new TestScheduler().With(scheduler =>
            {
                IEnumerable<GitRemoteRepository> repositories = new List<GitRemoteRepository>()
                {
                    new GitRemoteRepository(){Name = "abc"},
                    new GitRemoteRepository(){Name = "xyz"},
                };
                _gitClientService.Expect(x => x.GetAllRepositories()).Return(repositories.FromTaskAsync());

                var sut = CreateSut();
                sut.Initialize();

                sut.FilterRepoName = string.Empty;

                scheduler.AdvanceByMs(201);

                Assert.That(sut.FilteredRepositories.Count, Is.EqualTo(repositories.Count()));

                sut.FilterRepoName = "ab";

                scheduler.AdvanceByMs(201);

                Assert.That(sut.FilteredRepositories.Count, Is.EqualTo(1));
            });
        }

        [Test]
        public void FilterRepositories_FoundResults_ResultsShouldBeOrderedByName()
        {
            new TestScheduler().With(scheduler =>
            {
                IEnumerable<GitRemoteRepository> repositories = new List<GitRemoteRepository>()
                {
                    new GitRemoteRepository(){Name = "xyz"},
                    new GitRemoteRepository(){Name = "abd"},
                    new GitRemoteRepository(){Name = "abc"},
                };
                _gitClientService.Expect(x => x.GetAllRepositories()).Return(repositories.FromTaskAsync());

                var sut = CreateSut();
                sut.Initialize();

                sut.FilterRepoName = "ab";

                scheduler.AdvanceByMs(201);
                CollectionAssert.AreEqual(sut.FilteredRepositories, sut.FilteredRepositories.OrderBy(x => x.Name));
            });
        }

        private CloneRepositoriesDialogViewModel CreateSut()
        {
            return new CloneRepositoriesDialogViewModel(_gitClientService, _gitService, _fileService, _eventAggregatorService);
        }
    }
}
