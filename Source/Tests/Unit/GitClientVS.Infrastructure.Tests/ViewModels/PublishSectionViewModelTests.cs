using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using GitClientVS.Infrastructure.Tests.Extensions;
using GitClientVS.Infrastructure.ViewModels;
using GitClientVS.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace GitClientVS.Infrastructure.Tests.ViewModels
{
    [TestFixture]
    public class PublishSectionViewModelTests
    {
        private IGitClientService _gitClientService;
        private IGitService _gitService;

        private PublishSectionViewModel _sut;
        private IFileService _fileService;
        private IUserInformationService _userInfoService;
        private IGitWatcher _gitWatcher;

        [SetUp]
        public void SetUp()
        {
            _gitClientService = MockRepository.GenerateMock<IGitClientService>();
            _gitService = MockRepository.GenerateMock<IGitService>();
            _fileService = MockRepository.GenerateMock<IFileService>();
            _userInfoService = MockRepository.GenerateMock<IUserInformationService>();
            _gitWatcher = MockRepository.GenerateMock<IGitWatcher>();

            _sut = CreateSut();
        }

        [Test]
        public void Initialize_TeamsLoadedCorrectly_ShouldAssignTeamNamesAndCurrentUserToViewModel()
        {
            IEnumerable<GitTeam> teams = new List<GitTeam>() { new GitTeam() { Name = "TeamName" } };
            var connectionData = new ConnectionData() { UserName = "UserName" };

            _gitClientService.Expect(x => x.GetTeams()).Return(teams.FromTaskAsync());
            _userInfoService.Stub(x => x.ConnectionData).Return(connectionData);

            _sut.Initialize();

            CollectionAssert.AreEqual(new[] { "UserName", "TeamName" }, _sut.Owners);
            Assert.AreEqual("UserName", _sut.SelectedOwner);
        }

        [Test]
        public void Initialize_GetTeamsThrowsException_ShouldSetErrorMessage()
        {
            _sut.ErrorMessage = null;
            _gitClientService.Expect(x => x.GetTeams()).Throw(new Exception());

            _sut.Initialize();

            Assert.IsNotNull(_sut.ErrorMessage);
        }

        [Test]
        public void PublishCommand_CorrectParameters_ShouldBeEnabled()
        {
            IEnumerable<GitTeam> teams = new List<GitTeam>() { new GitTeam() { Name = "TeamName" } };
            var connectionData = new ConnectionData() { UserName = "UserName" };

            _gitClientService.Expect(x => x.GetTeams()).Return(teams.FromTaskAsync());
            _userInfoService.Stub(x => x.ConnectionData).Return(connectionData);

            _sut.Initialize();

            _sut.SelectedOwner = "owner";
            _sut.RepositoryName = "repoName";

            Assert.IsTrue(_sut.PublishRepositoryCommand.CanExecute(null));
        }

        [Test]
        public void PublishCommand_RepoIsEmpty_ShouldBeDisabled()
        {
            IEnumerable<GitTeam> teams = new List<GitTeam>() { new GitTeam() { Name = "TeamName" } };
            var connectionData = new ConnectionData() { UserName = "UserName" };

            _gitClientService.Expect(x => x.GetTeams()).Return(teams.FromTaskAsync());
            _userInfoService.Stub(x => x.ConnectionData).Return(connectionData);

            _sut.Initialize();

            _sut.SelectedOwner = string.Empty;
            _sut.RepositoryName = "repoName";

            Assert.IsFalse(_sut.PublishRepositoryCommand.CanExecute(null));
        }

        [Test]
        public void PublishCommand_Invoked_ShouldCreateAndPublishRepositoryAndRefreshActiveRepo()
        {
            IEnumerable<GitTeam> teams = new List<GitTeam>() { new GitTeam() { Name = "TeamName" } };
            var connectionData = new ConnectionData() { UserName = "UserName" };

            _gitClientService.Expect(x => x.GetTeams()).Return(teams.FromTaskAsync());
            _userInfoService.Stub(x => x.ConnectionData).Return(connectionData);

            _sut.Initialize();


            var remoteRepo = new GitRemoteRepository();

            _sut.SelectedOwner = "owner";
            _sut.RepositoryName = "repoName with space";
            _sut.Description = "description";
            _sut.IsPrivate = true;

            _gitClientService.Expect(x => x.CreateRepositoryAsync(Arg<GitRemoteRepository>.Matches(y =>
                    y.Name == "repoName-with-space" &&
                    y.Description == _sut.Description &&
                    y.IsPrivate == _sut.IsPrivate &&
                    y.Owner == _sut.SelectedOwner
                )))
                .Return(remoteRepo.FromTaskAsync());

            _gitService.Expect(x => x.PublishRepository(remoteRepo));
            _gitWatcher.Expect(x => x.Refresh());

            _sut.PublishRepositoryCommand.Execute(null);

            _gitClientService.VerifyAllExpectations();
            _gitService.VerifyAllExpectations();
            _gitWatcher.VerifyAllExpectations();
        }

        private PublishSectionViewModel CreateSut()
        {
            return new PublishSectionViewModel(
                _gitClientService,
                _gitService,
                _fileService,
                _userInfoService,
                _gitWatcher
            );
        }
    }
}
