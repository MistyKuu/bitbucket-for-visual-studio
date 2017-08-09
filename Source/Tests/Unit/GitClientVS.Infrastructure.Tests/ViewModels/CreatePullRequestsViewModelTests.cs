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
using GitClientVS.Contracts.Interfaces.ViewModels;
using GitClientVS.Contracts.Interfaces.Views;
using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using GitClientVS.Contracts.Models.Tree;
using GitClientVS.Infrastructure.Extensions;
using GitClientVS.Infrastructure.ViewModels;
using GitClientVS.Services;
using GitClientVS.Tests.Shared.Extensions;
using NUnit.Framework;
using ParseDiff;
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
        private IPullRequestDiffViewModel _pullRequestDiffViewModel;


        [SetUp]
        public void SetUp()
        {
            _gitClientService = MockRepository.GenerateMock<IGitClientService>();
            _gitService = MockRepository.GenerateMock<IGitService>();
            _pageNavigationService = MockRepository.GenerateMock<IPageNavigationService<IPullRequestsWindow>>();
            _eventAggregator = new EventAggregatorService();
            _treeStructureGenerator = MockRepository.GenerateMock<ITreeStructureGenerator>();
            _commandsService = MockRepository.GenerateMock<ICommandsService>();
            _pullRequestDiffViewModel = MockRepository.GenerateMock<IPullRequestDiffViewModel>();

            _pullRequestDiffViewModel.Stub(x => x.ThrowableCommands).Return(new List<ReactiveCommand>());

            _sut = CreateSut();
        }

        [Test]
        public void Initialize_CorrectSetup_BranchesShouldBeLoaded()
        {
            var remoteBranches = GetRemoteBranches();

            var activeRepository = GetActiveRepo();

            activeRepository.Branches.First(x => x.IsHead).TrackedBranchName = "RemoteHeadBranchName";

            _gitService.Expect(x => x.GetActiveRepository()).Return(activeRepository);
            _gitClientService.Expect(x => x.GetBranches()).Return(remoteBranches.FromTaskAsync());

            _sut.Initialize();

            Assert.That(_sut.SourceBranch, Is.EqualTo(remoteBranches.First(x => x.Name == "RemoteHeadBranchName")));
            Assert.That(_sut.DestinationBranch,
                Is.EqualTo(remoteBranches.First(x => x.Name == "RemoteDefaultBranchName")));
        }

        [Test]
        public void Initialize_PullRequestAlreadyExists_PullRequestDataShouldBeLoaded()
        {
            var remoteBranches = GetRemoteBranches();
            var activeRepository = GetActiveRepo();

            activeRepository.Branches.First(x => x.IsHead).TrackedBranchName = "RemoteHeadBranchName";

            var pullRequest = new GitPullRequest("Title", "Desc", new GitBranch() { Name = "SrcBranch" }, new GitBranch() { Name = "DestinationBranch" })
            {
                Author = new GitUser() { Username = "Author" },
                Reviewers = new Dictionary<GitUser, bool>()
                {
                    [new GitUser() { Username = "user" }] = true,
                    [new GitUser() { Username = "Author" }] = true
                }
            };
            IEnumerable<GitCommit> commits = new List<GitCommit>() { new GitCommit() };
            IEnumerable<FileDiff> fileDiffs = new List<FileDiff>() { new FileDiff() };
            List<ITreeFile> treeFiles = new List<ITreeFile>() { new TreeDirectory("name") };

            _gitService.Expect(x => x.GetActiveRepository()).Return(activeRepository);
            _gitClientService.Expect(x => x.GetBranches()).Return(remoteBranches.FromTaskAsync());

            var srcBranch = remoteBranches.First(x => x.Name == "RemoteHeadBranchName");
            var dstBranch = remoteBranches.First(x => x.Name == "RemoteDefaultBranchName");

            _gitClientService.Expect(x => x.GetPullRequestForBranches(srcBranch.Name, dstBranch.Name))
                .Return(pullRequest.FromTaskAsync());
            _gitClientService.Expect(x => x.GetCommitsRange(srcBranch, dstBranch)).Return(commits.FromTaskAsync());
            _gitClientService.Expect(x => x.GetCommitsDiff(srcBranch.Target.Hash, dstBranch.Target.Hash))
                .Return(fileDiffs.FromTaskAsync());
            _treeStructureGenerator.Expect(x => x.CreateFileTree(fileDiffs)).Return(treeFiles);

            _sut.Initialize();

            Assert.That(_sut.PullRequestDiffViewModel.Commits, Is.EqualTo(commits));
            Assert.That(_sut.PullRequestDiffViewModel.FilesTree, Is.EqualTo(treeFiles));
            Assert.That(_sut.PullRequestDiffViewModel.FileDiffs, Is.EqualTo(fileDiffs));

            Assert.That(_sut.Title, Is.EqualTo("Title"));
            Assert.That(_sut.Description, Is.EqualTo("Desc"));
            Assert.That(_sut.SelectedReviewers.Count, Is.EqualTo(1));
            Assert.That(_sut.RemotePullRequest, Is.EqualTo(pullRequest));
        }

        [Test]
        public void Initialize_PullRequestDoesntExist_PullRequestDataShouldBeLoadedFromDefaultsAndCommits()
        {
            var remoteBranches = GetRemoteBranches();
            var activeRepository = GetActiveRepo();

            activeRepository.Branches.First(x => x.IsHead).TrackedBranchName = "RemoteHeadBranchName";

            IEnumerable<GitCommit> commits = new List<GitCommit>() { new GitCommit() { Message = "Message" } };
            IEnumerable<FileDiff> fileDiffs = new List<FileDiff>() { new FileDiff() };
            List<ITreeFile> treeFiles = new List<ITreeFile>() { new TreeDirectory("name") };
            IEnumerable<GitUser> defaultReviewers = new List<GitUser>() { new GitUser(), new GitUser() };

            _pullRequestDiffViewModel.Stub(x => x.Commits).Return(commits.ToList());
            _pullRequestDiffViewModel.Stub(x => x.FileDiffs).Return(fileDiffs.ToList());
            _pullRequestDiffViewModel.Stub(x => x.FilesTree).Return(treeFiles);

            _gitService.Expect(x => x.GetActiveRepository()).Return(activeRepository);
            _gitClientService.Expect(x => x.GetBranches()).Return(remoteBranches.FromTaskAsync());

            var srcBranch = remoteBranches.First(x => x.Name == "RemoteHeadBranchName");
            var dstBranch = remoteBranches.First(x => x.Name == "RemoteDefaultBranchName");

            _gitClientService.Expect(x => x.GetPullRequestForBranches(srcBranch.Name, dstBranch.Name))
                .Return(Task.FromResult<GitPullRequest>(null));
            _gitClientService.Expect(x => x.GetCommitsRange(srcBranch, dstBranch)).Return(commits.FromTaskAsync());
            _gitClientService.Expect(x => x.GetCommitsDiff(srcBranch.Target.Hash, dstBranch.Target.Hash))
                .Return(fileDiffs.FromTaskAsync());
            _gitClientService.Expect(x => x.GetDefaultReviewers()).Return(defaultReviewers.FromTaskAsync());
            _treeStructureGenerator.Expect(x => x.CreateFileTree(fileDiffs)).Return(treeFiles);

            _sut.Initialize();

            Assert.That(_sut.PullRequestDiffViewModel.Commits, Is.EqualTo(commits));
            Assert.That(_sut.PullRequestDiffViewModel.FilesTree, Is.EqualTo(treeFiles));
            Assert.That(_sut.PullRequestDiffViewModel.FileDiffs, Is.EqualTo(fileDiffs));

            Assert.That(_sut.Title, Is.EqualTo(_sut.SourceBranch.Name));
            Assert.That(_sut.Description, Is.Not.Empty);
            Assert.That(_sut.SelectedReviewers.Count, Is.EqualTo(defaultReviewers.Count()));
            Assert.IsNull(_sut.RemotePullRequest);
        }

        [Test]
        public void Initialize_CurrentBranchIsLocalBranch_UserShouldBeNotified()
        {
           
            var remoteBranches = GetRemoteBranches();
            var activeRepository = GetActiveRepo();

            _gitService.Expect(x => x.GetActiveRepository()).Return(activeRepository);
            _gitClientService.Expect(x => x.GetBranches()).Return(remoteBranches.FromTaskAsync());

            _sut.Message = null;

            _sut.Initialize();


            Assert.That(_sut.SourceBranch, Is.EqualTo(remoteBranches.OrderBy(x => x.Name).First()));
            Assert.That(_sut.DestinationBranch, Is.EqualTo(remoteBranches.First(x => x.Name == "RemoteDefaultBranchName")));
            Assert.NotNull(_sut.Message);
        }


        [Test, Ignore("todo solve it later, it works differently when run from wpf app")]
        public void Initialize_GetPullRequestForBranchesThrowsException_ShouldSetErrorMessage()
        {
            var remoteBranches = GetRemoteBranches();
            var activeRepository = GetActiveRepo();

            _gitService.Expect(x => x.GetActiveRepository()).Return(activeRepository);
            _gitClientService.Expect(x => x.GetBranches()).Return(remoteBranches.FromTaskAsync());

            _gitClientService
                .Expect(x => x.GetPullRequestForBranches(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                .Throw(new Exception());

            _sut.ErrorMessage = null;

            _sut.Initialize();

            _gitClientService.VerifyAllExpectations();
            Assert.IsNotNull(_sut.ErrorMessage);
        }

        [Test]
        public void Initialize_GetActiveRepositoryThrowsException_ShouldSetErrorMessage()
        {
            var remoteBranches = GetRemoteBranches();
            _gitClientService.Expect(x => x.GetBranches()).Return(remoteBranches.FromTaskAsync());
            _gitService.Expect(x => x.GetActiveRepository()).Throw(new Exception());
            _sut.ErrorMessage = null;
            _sut.Initialize();

            Assert.IsNotNull(_sut.ErrorMessage);
        }

        [Test]
        public void CreatePullRequest_PullRequestDoesntExist_ShouldCreatePullRequestAndNavigateBack()
        {
            _sut.RemotePullRequest = null;
            _sut.Title = Guid.NewGuid().ToString();
            _sut.Description = Guid.NewGuid().ToString();
            _sut.SourceBranch = new GitBranch() { Name = Guid.NewGuid().ToString() };
            _sut.DestinationBranch = new GitBranch() { Name = Guid.NewGuid().ToString() };
            _sut.CloseSourceBranch = true;
            _sut.SelectedReviewers = new ReactiveList<GitUser>() { new GitUser() { Username = "User1" }, new GitUser() { Username = "User2" } };

            _pageNavigationService.Expect(x => x.NavigateBack(true));

            var result = _gitClientService
                .Capture()
                .Args<GitPullRequest>((s, pullRequest) => s.CreatePullRequest(pullRequest));

            _sut.CreateNewPullRequestCommand.Execute(null);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual(0, args.Id);
            Assert.AreEqual(_sut.Title, args.Title);
            Assert.AreEqual(_sut.Description, args.Description);
            Assert.AreEqual(_sut.SourceBranch.Name, args.SourceBranch.Name);
            Assert.AreEqual(_sut.DestinationBranch.Name, args.DestinationBranch.Name);
            Assert.AreEqual(_sut.CloseSourceBranch, args.CloseSourceBranch);
            Assert.AreEqual(_sut.SelectedReviewers.Count, args.Reviewers.Count);

            _pageNavigationService.VerifyAllExpectations();
        }

        [Test]
        public void CreatePullRequest_PullRequestExists_ShouldUpdatePullRequestAndNavigateBack()
        {
            _sut.RemotePullRequest = new GitPullRequest(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                new GitBranch() { Name = Guid.NewGuid().ToString() }, new GitBranch() { Name = Guid.NewGuid().ToString() })
            {
                Id = 15,
                Version = Guid.NewGuid().ToString()
            };
            _sut.Title = Guid.NewGuid().ToString();
            _sut.Description = Guid.NewGuid().ToString();
            _sut.SourceBranch = new GitBranch() { Name = Guid.NewGuid().ToString() };
            _sut.DestinationBranch = new GitBranch() { Name = Guid.NewGuid().ToString() };
            _sut.CloseSourceBranch = true;
            _sut.SelectedReviewers =
                new ReactiveList<GitUser>() { new GitUser() { Username = "User1" }, new GitUser() { Username = "User2" } };

            _pageNavigationService.Expect(x => x.NavigateBack(true));

            var result = _gitClientService
                .Capture()
                .Args<GitPullRequest>((s, pullRequest) => s.UpdatePullRequest(pullRequest));

            _sut.CreateNewPullRequestCommand.Execute(null);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual(_sut.RemotePullRequest.Id, args.Id);
            Assert.AreEqual(_sut.Title, args.Title);
            Assert.AreEqual(_sut.Description, args.Description);
            Assert.AreEqual(_sut.SourceBranch.Name, args.SourceBranch.Name);
            Assert.AreEqual(_sut.DestinationBranch.Name, args.DestinationBranch.Name);
            Assert.AreEqual(_sut.CloseSourceBranch, args.CloseSourceBranch);
            Assert.AreEqual(_sut.SelectedReviewers.Count, args.Reviewers.Count);
            Assert.AreEqual(_sut.RemotePullRequest.Version, args.Version);

            _pageNavigationService.VerifyAllExpectations();
        }

        [Test]
        public void CreatePullRequestCanExecute_CorrectParameters_ShouldBeEnabled()
        {
            using (_sut.SuppressChangeNotifications())
            {
                _sut.SourceBranch = new GitBranch() { Name = Guid.NewGuid().ToString() };
                _sut.DestinationBranch = new GitBranch() { Name = Guid.NewGuid().ToString() };
            }

            _sut.ForcePropertyValidation(nameof(_sut.SourceBranch));
            _sut.ForcePropertyValidation(nameof(_sut.DestinationBranch));

            _sut.Title = Guid.NewGuid().ToString();

            Assert.IsTrue(_sut.CreateNewPullRequestCommand.CanExecute(null));
        }

        [Test]
        public void CreatePullRequestCanExecute_IncorrectParameters_ShouldBeEnabled()
        {
            using (_sut.SuppressChangeNotifications())
            {
                _sut.SourceBranch = new GitBranch() { Name = Guid.NewGuid().ToString() };
                _sut.DestinationBranch = null;
            }

            _sut.ForcePropertyValidation(nameof(_sut.SourceBranch));
            _sut.ForcePropertyValidation(nameof(_sut.DestinationBranch));

            _sut.Title = Guid.NewGuid().ToString();

            Assert.IsFalse(_sut.CreateNewPullRequestCommand.CanExecute(null));
        }


        private static IEnumerable<GitBranch> GetRemoteBranches()
        {
            return new List<GitBranch>()
            {
                new GitBranch()
                {
                    Name = "RemoteHeadBranchName",
                    Target = new GitCommit() {Hash = Guid.NewGuid().ToString()}
                },
                new GitBranch()
                {
                    Name = "RemoteSecondBranchName",
                    Target = new GitCommit() {Hash = Guid.NewGuid().ToString()}
                },
                new GitBranch()
                {
                    Name = "RemoteDefaultBranchName",
                    IsDefault = true,
                    Target = new GitCommit() {Hash = Guid.NewGuid().ToString()}
                },
            };
        }

        private static GitRemoteRepository GetActiveRepo()
        {
            return new GitRemoteRepository
            {
                Branches = new List<GitLocalBranch>()
                {
                    new GitLocalBranch() {IsHead = true, Name = "HeadBranch"},
                    new GitLocalBranch()
                    {
                        IsHead = false,
                        Name = "SecondBranch",
                        TrackedBranchName = "RemoteSecondBranchName"
                    },
                }
            };
        }


        private CreatePullRequestsViewModel CreateSut()
        {
            return new CreatePullRequestsViewModel(
                _gitClientService,
                _gitService,
                _pageNavigationService,
                _eventAggregator,
                _treeStructureGenerator,
                _commandsService,
                new DataNotifier(),
                _pullRequestDiffViewModel
                );
        }
    }
}