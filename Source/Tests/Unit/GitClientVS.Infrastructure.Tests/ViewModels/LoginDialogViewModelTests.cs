using System;
using System.IO;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models;
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

            var result = _gitClientService
                .Capture()
                .Args<GitCredentials>((s, repo) => s.LoginAsync(repo));

            _sut.ConnectCommand.Execute(null);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual(_sut.Login, args.Login);
            Assert.AreEqual(_sut.Password, args.Password);
            Assert.AreEqual(null, args.Host);
            Assert.AreEqual(_sut.IsEnterprise, args.IsEnterprise);

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

            var result = _gitClientService
                .Capture()
                .Args<GitCredentials>((s, cred) => s.LoginAsync(cred));

            _sut.ConnectCommand.Execute(null);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual(_sut.Login, args.Login);
            Assert.AreEqual(_sut.Password, args.Password);
            Assert.AreEqual(new Uri(_sut.Host).ToString(), args.Host.ToString());
            Assert.AreEqual(_sut.IsEnterprise, args.IsEnterprise);

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
