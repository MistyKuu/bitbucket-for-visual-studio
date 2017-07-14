using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Rhino.Mocks;

namespace GitClientVS.Infrastructure.Tests.ViewModels
{
    [TestFixture]
    public class PullRequestsDetailViewModelTests
    {
        private IGitClientService _gitClientService;
        private IGitService _gitService;

        private IUserInformationService _userInfoService;
        private IMessageBoxService _messageBoxService;
        private IEventAggregatorService _eventAggregatorService;
        private ICommandsService _commandsService;
        private IVsTools _vsTools;
        private IPullRequestDiffViewModel _pullRequestDiffViewModel;

        [SetUp]
        public void SetUp()
        {
            _gitClientService = MockRepository.GenerateMock<IGitClientService>();
            _pullRequestDiffViewModel = MockRepository.GenerateMock<IPullRequestDiffViewModel>();
            _gitService = MockRepository.GenerateMock<IGitService>();
            _userInfoService = MockRepository.GenerateMock<IUserInformationService>();
            _messageBoxService = MockRepository.GenerateMock<IMessageBoxService>();
            _eventAggregatorService = new EventAggregatorService();
            _commandsService = MockRepository.GenerateMock<ICommandsService>();
            _vsTools = MockRepository.GenerateMock<IVsTools>();
        }

        [Test]
        public void Initialize_CorrectDataReturned_ShouldInitialize()
        {
            long id = 123;
            var pqAuthor = "Author";

            var pullRequest = new GitPullRequest("Title", "Description", new GitBranch(){Name="SourceBranch"}, new GitBranch() { Name = "DestinationBranch" })
            {
                Reviewers = new Dictionary<GitUser, bool>()
                {
                    [new GitUser() { Username = "User1" }] = true,
                    [new GitUser() { Username = "User2" }] = false,
                },
                Author = new GitUser() { Username = pqAuthor }
            };

            var connectionData = new ConnectionData()
            {
                UserName = pqAuthor,
            };

            var sut = CreateSutWithData(id, pullRequest, connectionData);
            sut.Initialize(id);

            Assert.AreEqual(pullRequest, sut.PullRequest);
            Assert.IsNotEmpty(sut.PullRequestDiffViewModel.CommentViewModel.CommentTree);
            Assert.IsNotEmpty(sut.PullRequestDiffViewModel.Commits);
            Assert.IsNotEmpty(sut.PullRequestDiffViewModel.FileDiffs);
            Assert.IsNotEmpty(sut.PullRequestDiffViewModel.FilesTree);

            Assert.IsTrue(sut.ActionCommands.Any(sutActionCommand => sutActionCommand.Label.Contains("Merge")));
            Assert.IsTrue(sut.ActionCommands.Any(sutActionCommand => sutActionCommand.Label.Contains("Decline")));
        }

        [Test]
        public void Initialize_UserIsReviewerAndHasntApproved_ShouldBeAllowedToApprove()
        {
            long id = 123;
            var pqAuthor = "Author";
            var currentUser = "CurrentUser";

            var pullRequest = new GitPullRequest("Title", "Description", new GitBranch() { Name = "SourceBranch" }, new GitBranch() { Name = "DestinationBranch" })
            {
                Reviewers = new Dictionary<GitUser, bool>()
                {
                    [new GitUser() { Username = "User1" }] = true,
                    [new GitUser() { Username = "User2" }] = false,
                    [new GitUser() { Username = currentUser }] = false,
                },
                Author = new GitUser() { Username = pqAuthor }
            };

            var connectionData = new ConnectionData()
            {
                UserName = currentUser,
            };

            var sut = CreateSutWithData(id, pullRequest, connectionData);
            sut.Initialize(id);

            Assert.IsTrue(sut.ActionCommands.Any(sutActionCommand => sutActionCommand.Label.Contains("Approve")));
        }

        [Test]
        public void Initialize_UserIsReviewerAndApproved_ShouldBeAllowedToUnapprove()
        {
            long id = 123;
            var pqAuthor = "Author";
            var currentUser = "CurrentUser";

            var pullRequest = new GitPullRequest("Title", "Description", new GitBranch() { Name = "SourceBranch" }, new GitBranch() { Name = "DestinationBranch" })
            {
                Reviewers = new Dictionary<GitUser, bool>()
                {
                    [new GitUser() { Username = "User1" }] = true,
                    [new GitUser() { Username = "User2" }] = false,
                    [new GitUser() { Username = currentUser }] = true,
                },
                Author = new GitUser() { Username = pqAuthor }
            };

            var connectionData = new ConnectionData()
            {
                UserName = currentUser,
            };

            var sut = CreateSutWithData(id, pullRequest, connectionData);
            sut.Initialize(id);

            Assert.IsTrue(sut.ActionCommands.Any(sutActionCommand => sutActionCommand.Label.Contains("Unapprove")));
        }

        [Test]
        public void Initialize_UserIsAuthorAndApproved_ShouldSetFlagForAuthor()
        {
            long id = 123;
            var currentUser = "CurrentUser";
            var pqAuthor = currentUser;

            var pullRequest = new GitPullRequest("Title", "Description", new GitBranch() { Name = "SourceBranch" }, new GitBranch() { Name = "DestinationBranch" })
            {
                Reviewers = new Dictionary<GitUser, bool>()
                {
                    [new GitUser() { Username = "User1" }] = true,
                    [new GitUser() { Username = "User2" }] = false,
                    [new GitUser() { Username = currentUser }] = true,
                },
                Author = new GitUser() { Username = pqAuthor }
            };

            var connectionData = new ConnectionData()
            {
                UserName = currentUser,
            };

            var sut = CreateSutWithData(id, pullRequest, connectionData);
            sut.Initialize(id);

            Assert.IsTrue(sut.HasAuthorApproved);
        }

        private PullRequestsDetailViewModel CreateSutWithData(
            long id,
            GitPullRequest pullRequest,
            ConnectionData connectionData
            )
        {
            IEnumerable<GitCommit> commits = new List<GitCommit>() { new GitCommit() };
            IEnumerable<GitComment> comments = new List<GitComment>() { new GitComment() };
            IEnumerable<FileDiff> filesDiff = new List<FileDiff>() { new FileDiff() };

            _userInfoService.Stub(x => x.ConnectionData).Return(connectionData);
            _userInfoService.Stub(x => x.CurrentTheme).Return(Theme.Light);
            _gitClientService.Expect(x => x.GetPullRequest(id)).Return(pullRequest.FromTaskAsync());
            _gitClientService.Expect(x => x.GetPullRequestComments(id)).Return(comments.FromTaskAsync());
            _gitClientService.Expect(x => x.GetPullRequestCommits(id)).Return(commits.FromTaskAsync());
            _gitClientService.Expect(x => x.GetPullRequestDiff(id)).Return(filesDiff.FromTaskAsync());

            _pullRequestDiffViewModel.Expect(x => x.AddFileDiffs(filesDiff));
            _pullRequestDiffViewModel.Expect(x => x.UpdateComments(id)).Return(comments.FromTaskAsync());

            return CreateSut();
        }

        private PullRequestsDetailViewModel CreateSut()
        {
            return new PullRequestsDetailViewModel(
                _gitClientService,
                _gitService,
                _commandsService,
                _userInfoService,
                _eventAggregatorService,
                _messageBoxService,
                new DataNotifier(),
                _pullRequestDiffViewModel
            );
        }
    }
}
