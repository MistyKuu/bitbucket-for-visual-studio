using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    public class PullRequestsMainViewModelTests
    {
        private IGitClientService _gitClientService;
        private PullRequestsMainViewModel _sut;
        private IPageNavigationService<IPullRequestsWindow> _pageNavigationService;

        [SetUp]
        public void SetUp()
        {
            _gitClientService = MockRepository.GenerateMock<IGitClientService>();
            _pageNavigationService = MockRepository.GenerateMock<IPageNavigationService<IPullRequestsWindow>>();

            _sut = CreateSut();
        }

        [Test]
        public void Initialize_DataReceived_ShouldLoadAuthorsAndPullRequests()
        {
            IEnumerable<GitUser> authors = new List<GitUser>() { new GitUser() { Username = "Author1" } };
            List<GitPullRequest> pullRequests = new List<GitPullRequest>()
            {
                new GitPullRequest("Title", "Description", new GitBranch(){Name="SourceBranch"}, new GitBranch() { Name = "DestinationBranch" }),
                new GitPullRequest("Title2", "Description", new GitBranch(){Name="SourceBranch"}, new GitBranch() { Name = "DestinationBranch" }),
            };
            var iterator = new PageIterator<GitPullRequest>() { Values = pullRequests };

            using (_sut.SuppressChangeNotifications())
            {
                _sut.SelectedStatus = GitPullRequestStatus.Open;
                _sut.SelectedAuthor = authors.First();
            }

            _gitClientService.Expect(x => x.GetPullRequestsAuthors()).Return(authors.FromTaskAsync());
            _gitClientService.Expect(x => x.GetPullRequestsPage(
                state: _sut.SelectedStatus,
                author: _sut.SelectedAuthor.Username,
                limit: 50,
                page: 1
                )).Return(iterator.FromTaskAsync());

            _sut.Initialize();

            Assert.AreEqual(2, _sut.GitPullRequests.Count);
            Assert.AreEqual(1, _sut.Authors.Count);
        }

        [Test]
        public void Initialize_LoadNextPageAfterInitialization_ShouldLoadExpectedPullRequests()
        {
            IEnumerable<GitUser> authors = new List<GitUser>() { new GitUser() { Username = "Author1" } };
            List<GitPullRequest> pullRequests = Enumerable
                .Repeat(new GitPullRequest("Title", "Description", new GitBranch() { Name = "SourceBranch" }, new GitBranch() { Name = "DestinationBranch" }), 100)
                .ToList();

            var iterator = new PageIterator<GitPullRequest>() { Values = pullRequests.Take(50).ToList(), Next = "2" };

            using (_sut.SuppressChangeNotifications())
            {
                _sut.SelectedStatus = GitPullRequestStatus.Open;
                _sut.SelectedAuthor = authors.First();
            }

            _gitClientService.Expect(x => x.GetPullRequestsAuthors()).Return(authors.FromTaskAsync());
            _gitClientService.Expect(x => x.GetPullRequestsPage(
                state: _sut.SelectedStatus,
                author: _sut.SelectedAuthor.Username,
                limit: 50,
                page: 1
            ))
            .Repeat.Once()
            .Return(iterator.FromTaskAsync());

            _sut.Initialize();

            Assert.AreEqual(50, _sut.GitPullRequests.Count);

            var iterator2 = new PageIterator<GitPullRequest>() { Values = pullRequests.Skip(50).Take(50).ToList(), Next = "2" };

            _gitClientService.Expect(x => x.GetPullRequestsPage(
                state: _sut.SelectedStatus,
                author: _sut.SelectedAuthor.Username,
                limit: 50,
                page: 2
            ))
            .Repeat.Once()
            .Return(iterator2.FromTaskAsync());


            _sut.LoadNextPageCommand.Execute(null);

            Assert.AreEqual(100, _sut.GitPullRequests.Count);
        }

        [Test]
        public void Initialize_SecondTime_ShouldntReinitializePullRequests()
        {
            Initialize_LoadNextPageAfterInitialization_ShouldLoadExpectedPullRequests();

            _sut.InitializeCommand.Execute(null);

            _gitClientService.VerifyAllExpectations();
        }

        [Test]
        public void RefreshCommand_Invoked_ShouldLoadPullRequestsFirstPage()
        {
            Initialize_LoadNextPageAfterInitialization_ShouldLoadExpectedPullRequests();

            List<GitPullRequest> pullRequests = Enumerable
                .Repeat(new GitPullRequest("Title", "Description", new GitBranch() { Name = "SourceBranch" }, new GitBranch() { Name = "DestinationBranch" }), 100)
                .ToList();

            var iterator = new PageIterator<GitPullRequest>() { Values = pullRequests.Take(50).ToList(), Next = "2" };

            _gitClientService.Expect(x => x.GetPullRequestsPage(
                state: _sut.SelectedStatus,
                author: _sut.SelectedAuthor.Username,
                limit: 50,
                page: 1
            ))
            .Repeat.Once()
            .Return(iterator.FromTaskAsync());

            _sut.RefreshPullRequestsCommand.Execute(null);

            Assert.AreEqual(50, _sut.GitPullRequests.Count);
        }


        private PullRequestsMainViewModel CreateSut()
        {
            return new PullRequestsMainViewModel(
                 _gitClientService,
                 _pageNavigationService,
                 new DataNotifier()
            );
        }
    }
}
