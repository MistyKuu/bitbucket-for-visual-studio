using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BitBucket.REST.API.Clients.Enterprise;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Mappings;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.QueryBuilders;
using GitClientVS.Tests.Shared.Extensions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RestSharp;
using Rhino.Mocks;

namespace Bitbucket.REST.API.Tests.Enterprise
{
    [TestFixture]
    public class EnterprisePullRequestsClientTests
    {
        private IEnterpriseBitbucketRestClient _restClient;
        private EnterprisePullRequestsClient _sut;

        [SetUp]
        public void SetUp()
        {
            _restClient = MockRepository.GenerateMock<IEnterpriseBitbucketRestClient>();
            var connection = new Connection(
                new Uri("http://url.com"),
                new Uri("http://api.url.com"),
                new Credentials("Login", "Password")
                );

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<EnterpriseToStandardMappingsProfile>();
            });

            _sut = new EnterprisePullRequestsClient(_restClient, connection);
        }

        [Test]
        public async Task ApprovePullRequest_ShouldReturnValidParticipant()
        {
            var enterpriseParticipant = new Fixture().Create<EnterpriseParticipant>();
            enterpriseParticipant.User.Links.Clone = null;

            var response = MockRepository.GenerateMock<IRestResponse<EnterpriseParticipant>>();
            response.Stub(x => x.Data).Return(enterpriseParticipant);

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse<EnterpriseParticipant>>((s, req) => s.ExecuteTaskAsync<EnterpriseParticipant>(req), response);

            var participant = await _sut.ApprovePullRequest("repoName", "owner", 1);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual("projects/owner/repos/repoName/pull-requests/1/approve", args.Resource);
            Assert.AreEqual(Method.POST, args.Method);

            Assert.AreEqual(enterpriseParticipant.Approved, participant.Approved);
            Assert.AreEqual(enterpriseParticipant.Role, participant.Role);
            Assert.AreEqual(enterpriseParticipant.User.Username, participant.User.Username);
            Assert.AreEqual(enterpriseParticipant.User.DisplayName, participant.User.DisplayName);
        }

        [Test]
        public async Task DisapprovePullRequest_ShouldCallCorrectUrlAndMethod()
        {
            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse>((s, req) => s.ExecuteTaskAsync(req), MockRepository.GenerateMock<IRestResponse>());

            await _sut.DisapprovePullRequest("repoName", "owner", 1);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual("projects/owner/repos/repoName/pull-requests/1/approve", args.Resource);
            Assert.AreEqual(Method.DELETE, args.Method);
        }

        [Test]
        public async Task CreatePullRequest_ShouldCallCorrectUrlAndMethod()
        {
            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse>((s, req) => s.ExecuteTaskAsync(req), MockRepository.GenerateMock<IRestResponse>());

            var pq = new PullRequest()
            {
                Id = 1,
                Title = "title",
                Description = "description",
                Source = new Source() { Branch = new Branch() { Name = "s" } },
                Destination = new Source() { Branch = new Branch() { Name = "d" } },
                CloseSourceBranch = true,
                Reviewers = new List<User>() { new User() { Username = "user", DisplayName = "displayname" } },
                Version = "1.0"
            };

            await _sut.CreatePullRequest(pq, "reponame", "owner");

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual($"projects/owner/repos/reponame/pull-requests", args.Resource);
            Assert.AreEqual(Method.POST, args.Method);

            var body = args.Parameters.First(p => p.Type == ParameterType.RequestBody);

            Assert.AreEqual(Utilities.LoadFile(Paths.GetEnterpriseDataPath("CreatePullRequestRequest.json")), body.Value);
        }

        [Test]
        public async Task DeclinePullRequest_ShouldCallCorrectUrlAndMethod()
        {
            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse>((s, req) => s.ExecuteTaskAsync(req), MockRepository.GenerateMock<IRestResponse>());

            await _sut.DeclinePullRequest("reponame", "owner", 1, "1.0");

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual($"projects/owner/repos/reponame/pull-requests/1/decline?version=1.0", args.Resource);
            Assert.AreEqual(Method.POST, args.Method);
        }

        [Test]
        public async Task GetAuthors_ShouldCallCorrectUrlAndResult()
        {
            var authors = new Fixture().CreateMany<EnterpriseUser>().ToList();

            foreach (var author in authors)
                author.Links.Clone = null;

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<EnterpriseUser>>((s, url, limit, queryString) => s.GetAllPages<EnterpriseUser>(url, limit, queryString), authors);

            var resultAuthors = (await _sut.GetAuthors("reponame", "owner")).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual($"projects/owner/repos/reponame/participants", args.arg1);
            Assert.AreEqual(100, args.arg2);
            Assert.IsNull(args.arg3);

            var firstResultAuthor = resultAuthors.First();
            var expectedAuthor = authors.First();

            Assert.AreEqual(authors.Count, resultAuthors.Count);
            Assert.AreEqual(expectedAuthor.DisplayName, firstResultAuthor.DisplayName);
            Assert.AreEqual(expectedAuthor.Username, firstResultAuthor.Username);
            Assert.AreEqual(expectedAuthor.Email, firstResultAuthor.Email);
            Assert.AreEqual(expectedAuthor.Links.Avatar.First().Href, firstResultAuthor.Links.Avatar.Href);
        }

        [Test]
        public async Task GetCommitsDiff_ShouldCallCorrectUrlAndResult()
        {
            var diffResponse = new Fixture().Create<EnterpriseDiffResponse>();

            var response = MockRepository.GenerateMock<IRestResponse<EnterpriseDiffResponse>>();
            response.Stub(x => x.Data).Return(diffResponse);

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse<EnterpriseDiffResponse>>(
                    (s, req) => s.ExecuteTaskAsync<EnterpriseDiffResponse>(req), response);

            var commitDiff = await _sut.GetCommitsDiff("repoName", "owner", "fromCommit", "toCommit");

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual($"projects/owner/repos/repoName/compare/diff", args.Resource);
            Assert.AreEqual(Method.GET, args.Method);
            Assert.AreEqual(args.Parameters[0].Name, "from");
            Assert.AreEqual(args.Parameters[0].Value, "fromCommit");
            Assert.AreEqual(args.Parameters[1].Name, "to");
            Assert.AreEqual(args.Parameters[1].Value, "toCommit");

            //todo generate diff from file and validate commit diff creation
        }

        [Test]
        public async Task GetDefaultReviewers_ShouldReturnEmptyListBecauseItsNotImplementedYet()
        {
            var reviewers = await _sut.GetDefaultReviewers("repoName", "owner");
            Assert.IsEmpty(reviewers);
        }

        [Test]
        public async Task GetPullRequest_ShouldReturnValidResult()
        {
            var pullRequest = new Fixture().Create<EnterprisePullRequest>();

            var response = MockRepository.GenerateMock<IRestResponse<EnterprisePullRequest>>();
            response.Stub(x => x.Data).Return(pullRequest);

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse<EnterprisePullRequest>>((s, req) => s.ExecuteTaskAsync<EnterprisePullRequest>(req), response);

            var resultPullRequest = await _sut.GetPullRequest("repoName", "owner", 1);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual($"projects/owner/repos/repository/pull-requests/1", args.Resource);
            Assert.AreEqual(Method.POST, args.Method);

            Assert.AreEqual(pullRequest.CreatedOn, resultPullRequest.CreatedOn);
        }

        [Test]
        public async Task GetPullRequestComments_ShouldCallCorrectUrlAndGetResult()
        {
            var inputData = new Fixture().CreateMany<EnterpriseCommentActivity>().ToList();

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<EnterpriseCommentActivity>>((s, url, limit, queryString) => s.GetAllPages<EnterpriseCommentActivity>(url, limit, queryString), inputData);

            var resultData = (await _sut.GetPullRequestComments("reponame", "owner", 1)).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual($"projects/owner/repos/repositoryName/pull-requests/id/activities", args.arg1);
            Assert.AreEqual(50, args.arg2);
            Assert.IsNull(args.arg3);

            Assert.AreEqual(inputData.Count, resultData.Count);
        }

        [Test]
        public async Task GetPullRequestCommits_ShouldCallCorrectUrlAndGetResult()
        {
            var inputData = new Fixture().CreateMany<EnterpriseCommit>().ToList();

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<EnterpriseCommit>>((s, url, limit, queryString) => s.GetAllPages<EnterpriseCommit>(url, limit, queryString), inputData);

            var resultData = (await _sut.GetPullRequestCommits("reponame", "owner", 1)).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual($"projects/owner/repos/repositoryName/pull-requests/id/commits", args.arg1);
            Assert.AreEqual(50, args.arg2);
            Assert.IsNull(args.arg3);

            Assert.AreEqual(inputData.Count, resultData.Count);
        }

        [Test]
        public async Task GetPullRequestDiff_ShouldCallCorrectUrlAndResult()
        {
            var inputData = new Fixture().Create<EnterpriseDiffResponse>();

            var response = MockRepository.GenerateMock<IRestResponse<EnterpriseDiffResponse>>();
            response.Stub(x => x.Data).Return(inputData);

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse<EnterpriseDiffResponse>>(
                    (s, req) => s.ExecuteTaskAsync<EnterpriseDiffResponse>(req), response);

            var resultData = await _sut.GetPullRequestDiff("repoName", "owner", 1);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual($"projects/owner/repos/repoName/compare/diff", args.Resource);
            Assert.AreEqual(Method.GET, args.Method);
        }

        [Test]
        public async Task GetPullRequestForBranches_ShouldCallCorrectUrlAndGetResult()
        {
            var inputData = new Fixture().CreateMany<EnterprisePullRequest>().ToList();//todo only one

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<EnterprisePullRequest>>((s, url, limit, queryString) => s.GetAllPages<EnterprisePullRequest>(url, limit, queryString), inputData);

            var resultData = (await _sut.GetPullRequestForBranches("reponame", "owner", "sourceBranch", "destBranch"));

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual($"projects/owner/repos/repositoryName/pull-requests/id/commits", args.arg1);
            Assert.AreEqual(50, args.arg2);
            Assert.IsNull(args.arg3);
        }

        [Test]
        public async Task GetPullRequestsPage_ShouldCallCorrectUrlAndResult()
        {
            var inputData = new Fixture().Create<EnterpriseIteratorBasedPage<EnterprisePullRequest>>();

            var response = MockRepository.GenerateMock<IRestResponse<EnterpriseIteratorBasedPage<EnterprisePullRequest>>>();
            response.Stub(x => x.Data).Return(inputData);

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse<EnterpriseIteratorBasedPage<EnterprisePullRequest>>>(
                    (s, req) => s.ExecuteTaskAsync<EnterpriseIteratorBasedPage<EnterprisePullRequest>>(req), response);

            var resultData = await _sut.GetPullRequestsPage("repoName", "owner", 1);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual($"projects/owner/repos/repoName/compare/diff", args.Resource);
            Assert.AreEqual(Method.GET, args.Method);
        }

        [Test]
        public async Task GetRepositoryUsers_ShouldCallCorrectUrlAndGetResult()
        {
            var inputData = new Fixture().CreateMany<EnterpriseUser>().ToList();

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<EnterpriseUser>>((s, url, limit, queryString) => s.GetAllPages<EnterpriseUser>(url, limit, queryString), inputData);

            var resultData = (await _sut.GetRepositoryUsers("reponame", "owner", "filter")).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual($"projects/owner/repos/repositoryName/pull-requests/id/commits", args.arg1);
            Assert.AreEqual(50, args.arg2);
            Assert.IsNull(args.arg3);

            Assert.AreEqual(inputData.Count, resultData.Count);
        }

        [Test]
        public async Task UpdatePullRequest_ShouldCallCorrectUrlAndMethod()
        {
            var pullRequest = new Fixture().Create<PullRequest>();

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse>((s, req) => s.ExecuteTaskAsync(req), MockRepository.GenerateMock<IRestResponse>());

            await _sut.UpdatePullRequest(pullRequest, "repo", "owner");

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual("projects/owner/repos/repoName/pull-requests/1/approve", args.Resource);
            Assert.AreEqual(Method.DELETE, args.Method);
        }

        [Test]
        public async Task MergePullRequest_ShouldCallCorrectUrlAndMethod()
        {
            var request = new Fixture().Create<MergeRequest>();

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse>((s, req) => s.ExecuteTaskAsync(req), MockRepository.GenerateMock<IRestResponse>());

            await _sut.MergePullRequest("repo", "owner", request);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual("projects/owner/repos/repoName/pull-requests/1/approve", args.Resource);
            Assert.AreEqual(Method.DELETE, args.Method);
        }

        [Test]
        public void GetPullRequestQueryBuilder_ShouldReturnCorrectQueryParams()
        {
             _sut.GetPullRequestQueryBuilder(); //todo
        }

    }
}
