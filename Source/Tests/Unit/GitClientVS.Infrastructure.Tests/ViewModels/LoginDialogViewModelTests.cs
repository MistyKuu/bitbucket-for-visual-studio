using System;
using System.IO;
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
    public class LoginDialogViewModelTests
    {
        private IGitClientService _gitClientService;
        private LoginDialogViewModel _sut;


        [SetUp]
        public void SetUp()
        {
            _gitClientService = MockRepository.GenerateMock<IGitClientService>();

            _sut = CreateSut();
        }

        [Test]
        public void ConnectCommand_CorrectEnterpriseParameters_ShouldBeEnabled()
        {
            _sut.Initialize();

            _sut.IsEnterprise = true;
            _sut.Login = Guid.NewGuid().ToString();
            _sut.Password = Guid.NewGuid().ToString();
            _sut.Host = "http://test.com";

            Assert.IsTrue(_sut.ConnectCommand.CanExecute(null));
        }

        [Test]
        public void ConnectCommand_EnterpriseParametersNoHost_ShouldBeDisabled()
        {
            _sut.Initialize();

            _sut.IsEnterprise = true;
            _sut.Login = Guid.NewGuid().ToString();
            _sut.Password = Guid.NewGuid().ToString();

            Assert.IsFalse(_sut.ConnectCommand.CanExecute(null));
        }

        [Test]
        public void ConnectCommand_EnterpriseParametersInvalidHost_ShouldBeDisabled()
        {
            _sut.Initialize();

            _sut.IsEnterprise = true;
            _sut.Login = Guid.NewGuid().ToString();
            _sut.Password = Guid.NewGuid().ToString();
            _sut.Host = "InvalidHost";

            Assert.IsFalse(_sut.ConnectCommand.CanExecute(null));
        }

        [Test]
        public void ConnectCommand_CorrectStandardParameters_ShouldBeEnabled()
        {
            _sut.Initialize();

            _sut.IsEnterprise = false;
            _sut.Login = Guid.NewGuid().ToString();
            _sut.Password = Guid.NewGuid().ToString();

            Assert.IsTrue(_sut.ConnectCommand.CanExecute(null));
        }

        [Test]
        public void ConnectCommand_StandardParametersNoHost_ShouldBeEnabled()
        {
            _sut.Initialize();

            _sut.IsEnterprise = false;
            _sut.Login = Guid.NewGuid().ToString();
            _sut.Password = Guid.NewGuid().ToString();

            Assert.IsTrue(_sut.ConnectCommand.CanExecute(null));
        }

        [Test]
        public void ConnectCommand_InvokedForStandard_ShouldLogin()
        {
            _sut.Initialize();

            _sut.IsEnterprise = false;
            _sut.Login = Guid.NewGuid().ToString();
            _sut.Password = Guid.NewGuid().ToString();

            _gitClientService.Expect(x => x.LoginAsync(Arg<GitCredentials>.Matches(y =>
                y.Login == _sut.Login &&
                y.Password == _sut.Password &&
                y.Host == null &&
                y.IsEnterprise == _sut.IsEnterprise
            )));

            _sut.ConnectCommand.Execute(null);

            _gitClientService.VerifyAllExpectations();
        }

        [Test]
        public void ConnectCommand_InvokedForEnterprise_ShouldLogin()
        {
            _sut.Initialize();

            _sut.IsEnterprise = true;
            _sut.Login = Guid.NewGuid().ToString();
            _sut.Password = Guid.NewGuid().ToString();
            _sut.Host = "http://abc.com";

            _gitClientService.Expect(x => x.LoginAsync(Arg<GitCredentials>.Matches(y =>
                y.Login == _sut.Login &&
                y.Password == _sut.Password &&
                y.Host.ToString() == new Uri(_sut.Host).ToString() &&
                y.IsEnterprise == _sut.IsEnterprise
            )));

            _sut.ConnectCommand.Execute(null);

            _gitClientService.VerifyAllExpectations();
        }

        private LoginDialogViewModel CreateSut()
        {
            return new LoginDialogViewModel(
                _gitClientService
            );
        }
    }
}
