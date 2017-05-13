using System;
using System.IO;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using GitClientVS.Infrastructure.ViewModels;
using GitClientVS.Services;
using GitClientVS.Tests.Shared.Extensions;
using NUnit.Framework;
using Rhino.Mocks;

namespace GitClientVS.Infrastructure.Tests.ViewModels
{
    [TestFixture]
    public class CreateRepositoriesDialogViewModelTests
    {
        private IGitClientService _gitClientService;
        private IGitService _gitService;
        private CreateRepositoriesDialogViewModel _sut;
        private IFileService _fileService;


        [SetUp]
        public void SetUp()
        {
            _gitClientService = MockRepository.GenerateMock<IGitClientService>();
            _gitService = MockRepository.GenerateMock<IGitService>();
            _fileService = MockRepository.GenerateMock<IFileService>();


            _sut = CreateSut();
        }

        [Test]
        public void OnCreate_DefaultLocalPathIsCorrect_ShouldBeUsed()
        {
            _gitService.Expect(x => x.GetDefaultRepoPath()).Return("DefaultRepoPath");
            var sut = CreateSut();
            Assert.That(sut.LocalPath, Is.EqualTo("DefaultRepoPath"));
        }

        [Test]
        public void OnCreate_DefaultLocalPathIsInCorrect_ShouldUseDefaultRepoPath()
        {
            _gitService.Expect(x => x.GetDefaultRepoPath()).Return(string.Empty);
            var sut = CreateSut();
            Assert.That(sut.LocalPath, Is.EqualTo(Paths.DefaultRepositoryPath));
        }

        [Test]
        public void CreateCommand_InvalidParameters_ShouldBeDisabled()
        {
            _sut.Name = null;

            Assert.That(_sut.CreateCommand.CanExecute(null), Is.EqualTo(false));
        }

        [Test]
        public void CreateCommand_CorrectParameters_ShouldBeEnabled()
        {
            const string path = "CorrectPath";
            const string repoName = "repoName";

            _fileService.Expect(x => x.IsPath(path)).Return(true);
            _fileService.Expect(x => x.Exists(Path.Combine(path, repoName))).Return(false);

            _sut.Name = repoName;
            _sut.LocalPath = path;

            Assert.That(_sut.CreateCommand.CanExecute(null), Is.EqualTo(true));
        }

        [Test]
        public void CreateCommand_Invoked_ShouldCreateRepositoryAndCloneAndRaiseClosed()
        {
            bool closed = false;

            _sut.Name = "repo";
            _sut.LocalPath = "path";
            _sut.IsPrivate = true;
            _sut.Closed += delegate { closed = true; };

            var remoteRepo = new GitRemoteRepository()
            {
                CloneUrl = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString()
            };

            _gitService.Expect(x => x.CloneRepository(remoteRepo.CloneUrl, remoteRepo.Name, _sut.LocalPath));

            var result = _gitClientService
                .Capture()
                .Args<GitRemoteRepository, GitRemoteRepository>((s, repo) => s.CreateRepositoryAsync(repo), remoteRepo);

            _sut.CreateCommand.Execute(null);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual(_sut.Name, args.Name);
            Assert.AreEqual(_sut.Description, args.Description);
            Assert.AreEqual(_sut.IsPrivate, args.IsPrivate);

            _gitClientService.VerifyAllExpectations();
            _gitService.VerifyAllExpectations();
            Assert.That(closed, Is.EqualTo(true));
        }

        [Test]
        public void CreateCommand_InvokedAndRaisedException_ShouldSetErrorMessage()
        {
            _gitClientService.Expect(x => x.CreateRepositoryAsync(Arg<GitRemoteRepository>.Is.Anything))
                .Throw(new Exception());

            _sut.Initialize();
            _sut.ErrorMessage = null;
            _sut.CreateCommand.Execute(null);
            Assert.IsNotNull(_sut.ErrorMessage);
        }

        private CreateRepositoriesDialogViewModel CreateSut()
        {
            return new CreateRepositoriesDialogViewModel(
                _gitClientService,
                _gitService,
                _fileService
                );
        }

    }
}
