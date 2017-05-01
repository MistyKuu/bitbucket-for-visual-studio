using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GitClientVS.Contracts.Events;
using GitClientVS.Contracts.Interfaces;
using GitClientVS.Contracts.Interfaces.Services;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Infrastructure.Extensions;
using GitClientVS.Infrastructure.Tests.Extensions;
using GitClientVS.Infrastructure.ViewModels;
using GitClientVS.Services;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoRhinoMock;
using ReactiveUI;
using ReactiveUI.Testing;
using Rhino.Mocks;

namespace GitClientVS.Infrastructure.Tests.ViewModels
{
    [TestFixture]
    public class CreatePullRequestsViewModelTests
    {
        private IGitClientService _gitClientService;
        private IGitService _gitService;
        private IPageNavigationService<IPullRequestsWindow> _pageNavigationService;
        private IEventAggregatorService _eventAggregator;
        private ITreeStructureGenerator _treeStructureGenerator;
        private ICommandsService _commandsService;
        private CreatePullRequestsViewModel _sut;


        [SetUp]
        public void SetUp()
        {
            _gitClientService = MockRepository.GenerateMock<IGitClientService>();
            _gitService = MockRepository.GenerateMock<IGitService>();
            _pageNavigationService = MockRepository.GenerateMock<IPageNavigationService<IPullRequestsWindow>>();
            _eventAggregator = new EventAggregatorService();
            _treeStructureGenerator = MockRepository.GenerateMock<ITreeStructureGenerator>();
            _commandsService = MockRepository.GenerateMock<ICommandsService>();


            _sut = CreateSut();
        }

        [Test]
        public void Initialize_CorrectSetup_BranchesShouldBeLoaded()
        {
            IEnumerable<GitBranch> remoteBranches = new List<GitBranch>()
            {
                new GitBranch(){Name = "RemoteHeadBranchName"},
                new GitBranch(){Name = "RemoteSecondBranchName"},
                new GitBranch() { Name = "RemoteDefaultBranchName", IsDefault = true},
            };
            var activeRepository = new GitRemoteRepository
            {
                Branches = new List<GitLocalBranch>()
                {
                    new GitLocalBranch() { IsHead = true, Name = "HeadBranch",TrackedBranchName = "RemoteHeadBranchName"},
                    new GitLocalBranch() { IsHead = false, Name = "SecondBranch",TrackedBranchName = "RemoteSecondBranchName"},
                }
            };

            _gitService.Expect(x => x.GetActiveRepository()).Return(activeRepository);
            _gitClientService.Expect(x => x.GetBranches()).Return(remoteBranches.FromTaskAsync());

            _sut.Initialize();

            Assert.That(_sut.SourceBranch, Is.EqualTo(remoteBranches.First(x => x.Name == "RemoteHeadBranchName")));
            Assert.That(_sut.DestinationBranch, Is.EqualTo(remoteBranches.First(x => x.Name == "RemoteDefaultBranchName")));
        }

        [Test]
        public void Initialize_CurrentBranchIsLocalBranch_UserShouldBeNotified()
        {
            IEnumerable<GitBranch> remoteBranches = new List<GitBranch>()
            {
                new GitBranch(){Name = "RemoteHeadBranchName"},
                new GitBranch(){Name = "RemoteSecondBranchName"},
                new GitBranch() { Name = "RemoteDefaultBranchName", IsDefault = true},
            };
            var activeRepository = new GitRemoteRepository
            {
                Branches = new List<GitLocalBranch>()
                {
                    new GitLocalBranch() { IsHead = true, Name = "HeadBranch"},
                    new GitLocalBranch() { IsHead = false, Name = "SecondBranch",TrackedBranchName = "RemoteSecondBranchName"},
                }
            };

            _gitService.Expect(x => x.GetActiveRepository()).Return(activeRepository);
            _gitClientService.Expect(x => x.GetBranches()).Return(remoteBranches.FromTaskAsync());

            _sut.Message = null;
            _sut.Initialize();

            Assert.That(_sut.SourceBranch, Is.EqualTo(remoteBranches.OrderBy(x => x.Name).First()));
            Assert.That(_sut.DestinationBranch, Is.EqualTo(remoteBranches.First(x => x.Name == "RemoteDefaultBranchName")));
            Assert.NotNull(_sut.Message);
        }

        private CreatePullRequestsViewModel CreateSut()
        {
            return new CreatePullRequestsViewModel(
                _gitClientService,
                _gitService,
                _pageNavigationService,
                _eventAggregator,
                _treeStructureGenerator,
                _commandsService);
        }

    }
}
