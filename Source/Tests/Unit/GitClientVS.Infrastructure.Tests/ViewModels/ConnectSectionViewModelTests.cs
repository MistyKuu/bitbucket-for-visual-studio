using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using GitClientVS.Infrastructure.ViewModels;
using GitClientVS.Services;
using GitClientVS.Tests.Shared.Extensions;
using NUnit.Framework;
using ReactiveUI.Testing;
using Rhino.Mocks;

namespace GitClientVS.Infrastructure.Tests.ViewModels
{
    [TestFixture]
    public class ConnectSectionViewModelTests
    {
        private ILoginDialogView _loginDialogView;
        private ICloneRepositoriesDialogView _cloneRepositoriesDialogView;
        private ICreateRepositoriesDialogView _createRepositoriesDialogView;
        private IEventAggregatorService _eventAggregatorService;
        private IUserInformationService _userInformationService;
        private IGitClientService _gitClientService;
        private IVsTools _vsTools;
        private ITeamExplorerCommandsService _teamExplorerCommandsService;
        private IGitService _gitService;

        [SetUp]
        public void SetUp()
        {
            _loginDialogView = MockRepository.GenerateMock<ILoginDialogView>();
            _cloneRepositoriesDialogView = MockRepository.GenerateMock<ICloneRepositoriesDialogView>();
            _createRepositoriesDialogView = MockRepository.GenerateMock<ICreateRepositoriesDialogView>();
            _eventAggregatorService = new EventAggregatorService();
            _userInformationService = MockRepository.GenerateMock<IUserInformationService>();
            _gitClientService = MockRepository.GenerateMock<IGitClientService>();
            _vsTools = MockRepository.GenerateMock<IVsTools>();
            _teamExplorerCommandsService = MockRepository.GenerateMock<ITeamExplorerCommandsService>();
            _gitService = MockRepository.GenerateMock<IGitService>();
        }

        [Test]
        public void OpenLoginCommand_ShouldOpenLoginWindow()
        {
            _loginDialogView.Expect(x => x.ShowDialog()).Return(true).Repeat.Once();

            var sut = CreateSut();
            sut.OpenLoginCommand.Execute(null);

            _loginDialogView.VerifyAllExpectations();
        }

        [Test]
        public void CloneRepositoriesCommand_ShouldOpenCloneWindow()
        {
            _cloneRepositoriesDialogView.Expect(x => x.ShowDialog()).Return(true).Repeat.Once();

            var sut = CreateSut();
            sut.OpenCloneCommand.Execute(null);

            _cloneRepositoriesDialogView.VerifyAllExpectations();
        }

        [Test]
        public void CreateRepositoriesCommand_ShouldOpenCreateRepositoriesWindow()
        {
            _createRepositoriesDialogView.Expect(x => x.ShowDialog()).Return(true).Repeat.Once();

            var sut = CreateSut();
            sut.OpenCreateCommand.Execute(null);

            _createRepositoriesDialogView.VerifyAllExpectations();
        }

        [Test]
        public void LogoutCommand_ShouldLogoutGitClient()
        {
            _gitClientService.Expect(x => x.Logout()).Repeat.Once();

            var sut = CreateSut();

            sut.LogoutCommand.Execute(null);

            _gitClientService.VerifyAllExpectations();
        }



        private ConnectSectionViewModel CreateSut()
        {
            return new ConnectSectionViewModel(
                _loginDialogView.CreateFactoryMock(),
                _cloneRepositoriesDialogView.CreateFactoryMock(),
                _createRepositoriesDialogView.CreateFactoryMock(),
                _eventAggregatorService,
                _userInformationService,
                _gitClientService,
                _gitService,
                _vsTools,
                _teamExplorerCommandsService
                );
        }

    }
}
