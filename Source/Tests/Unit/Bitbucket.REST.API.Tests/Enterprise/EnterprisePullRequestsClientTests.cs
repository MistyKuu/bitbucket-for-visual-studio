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
using BitBucket.REST.API.Serializers;
using GitClientVS.Tests.Shared.Extensions;
using NUnit.Framework;
using ParseDiff;
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
            var responseJson = Utilities.LoadFile(Paths.GetEnterpriseDataPath("ApprovePullRequestResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<EnterpriseParticipant>(responseJson);

            var response = MockRepository.GenerateMock<IRestResponse<EnterpriseParticipant>>();
            response.Stub(x => x.Data).Return(responseData);

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse<EnterpriseParticipant>>((s, req) => s.ExecuteTaskAsync<EnterpriseParticipant>(req), response);

            var participant = await _sut.ApprovePullRequest("repoName", "owner", 1);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("projects/owner/repos/repoName/pull-requests/1/approve", args.Resource);
                Assert.AreEqual(Method.POST, args.Method);

                Assert.AreEqual(true, participant.Approved);
                Assert.AreEqual("REVIEWER", participant.Role);
                Assert.AreEqual("TestUser", participant.User.Username);
                Assert.AreEqual("TestUser", participant.User.DisplayName);
            });
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
                Title = "master",
                Description = "description",
                Source = new Source() { Branch = new Branch() { Name = "master", IsDefault = false } },
                Destination = new Source() { Branch = new Branch() { Name = "4", IsDefault = false } },
                Reviewers = new List<User>() { new User() { Username = "MistyK" } },
            };

            await _sut.CreatePullRequest(pq, "reponame", "owner");

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual($"projects/owner/repos/reponame/pull-requests", args.Resource);
            Assert.AreEqual(Method.POST, args.Method);

            var body = args.Parameters.First(p => p.Type == ParameterType.RequestBody);

            var expectedJsonBody = Utilities.LoadFile(Paths.GetEnterpriseDataPath("CreatePullRequestRequest.json"));

            Assert.AreEqual(expectedJsonBody, body.Value.ToString());
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
            var responseJson = Utilities.LoadFile(Paths.GetEnterpriseDataPath("GetAuthorsResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<EnterpriseIteratorBasedPage<EnterpriseUser>>(responseJson);

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<EnterpriseUser>>((s, url, limit, queryString) => s.GetAllPages<EnterpriseUser>(url, limit, queryString), responseData.Values);

            var resultAuthors = (await _sut.GetAuthors("reponame", "owner")).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual($"projects/owner/repos/reponame/participants", args.arg1);
            Assert.AreEqual(100, args.arg2);
            Assert.IsNull(args.arg3);

            var firstResultAuthor = resultAuthors.First();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(responseData.Values.Count, resultAuthors.Count);
                Assert.AreEqual("MistyKuuuuuuuu", firstResultAuthor.DisplayName);
                Assert.AreEqual("MistyK", firstResultAuthor.Username);
                Assert.AreEqual("mistyksu@gmail.com", firstResultAuthor.Email);
                Assert.AreEqual("http://localhost:7990/users/mistyk", firstResultAuthor.Links.Self.Href);

            });
        }

        [Test]
        public async Task GetCommitsDiff_ShouldCallCorrectUrlAndResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetEnterpriseDataPath("GetCommitsDiffResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<EnterpriseDiffResponse>(responseJson);

            var response = MockRepository.GenerateMock<IRestResponse<EnterpriseDiffResponse>>();
            response.Stub(x => x.Data).Return(responseData);

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse<EnterpriseDiffResponse>>(
                    (s, req) => s.ExecuteTaskAsync<EnterpriseDiffResponse>(req), response);

            var commitDiff = await _sut.GetCommitsDiff("repoName", "owner", "9f0a336dc13893f8a73693267662a1b5835a3d87", "9f0a336dc13893f8a73693267662a1b5835a3d87");

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];
            Assert.Multiple(() =>
            {
                Assert.AreEqual($"projects/owner/repos/repoName/compare/diff", args.Resource);
                Assert.AreEqual(Method.GET, args.Method);
                Assert.AreEqual(args.Parameters[0].Name, "from");
                Assert.AreEqual(args.Parameters[0].Value, "9f0a336dc13893f8a73693267662a1b5835a3d87");
                Assert.AreEqual(args.Parameters[1].Name, "to");
                Assert.AreEqual(args.Parameters[1].Value, "9f0a336dc13893f8a73693267662a1b5835a3d87");

                var firstDiff = commitDiff.First();

                Assert.AreEqual(FileChangeType.Modified, firstDiff.Type);
                Assert.AreEqual(0, firstDiff.Deletions);
                Assert.AreEqual(0, firstDiff.Additions);
                Assert.AreEqual(1, firstDiff.Id);
                Assert.AreEqual("new2.txt", firstDiff.From);
                Assert.AreEqual("new4.txt", firstDiff.To);
                Assert.AreEqual("new2.txt", firstDiff.DisplayFileName);
                Assert.AreEqual(0, firstDiff.Chunks.Count);

            });
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
            var responseJson = Utilities.LoadFile(Paths.GetEnterpriseDataPath("GetPullRequestResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<EnterprisePullRequest>(responseJson);

            var response = MockRepository.GenerateMock<IRestResponse<EnterprisePullRequest>>();
            response.Stub(x => x.Data).Return(responseData);

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse<EnterprisePullRequest>>((s, req) => s.ExecuteTaskAsync<EnterprisePullRequest>(req), response);

            var resultPullRequest = await _sut.GetPullRequest("repoName", "owner", 1);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("projects/owner/repos/repoName/pull-requests/1", args.Resource);
                Assert.AreEqual(Method.GET, args.Method);

                Assert.AreEqual("04/03/2017 19:43:51", resultPullRequest.CreatedOn);
                Assert.AreEqual("04/03/2017 19:43:51", resultPullRequest.UpdatedOn);
                Assert.AreEqual(null, resultPullRequest.CloseSourceBranch);
                Assert.AreEqual("* asd\r\n* asd", resultPullRequest.Description);
                Assert.AreEqual("cccccccccccc", resultPullRequest.Title);
                Assert.AreEqual(48, resultPullRequest.Id);
                Assert.AreEqual(PullRequestOptions.OPEN, resultPullRequest.State);
                Assert.AreEqual("refs/heads/cccccccccccc", resultPullRequest.Source.Branch.Name);
                Assert.AreEqual("refs/heads/12", resultPullRequest.Destination.Branch.Name);
            });
        }

        [Test]
        public async Task GetPullRequestComments_ShouldCallCorrectUrlAndGetResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetEnterpriseDataPath("GetPullRequestCommentsResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<EnterpriseIteratorBasedPage<EnterpriseCommentActivity>>(responseJson);

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<EnterpriseCommentActivity>>((s, url, limit, queryString) => s.GetAllPages<EnterpriseCommentActivity>(url, limit, queryString), responseData.Values);

            var resultData = (await _sut.GetPullRequestComments("reponame", "owner", 1)).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("projects/owner/repos/reponame/pull-requests/1/activities", args.arg1);
                Assert.AreEqual(50, args.arg2);
                Assert.IsNull(args.arg3);

                var secondComment = resultData[1];

                Assert.AreEqual("ReplyComment", secondComment.Content.Html);
                Assert.AreEqual(null, secondComment.Content.Raw);
                Assert.AreEqual("05/14/2017 15:52:59", secondComment.CreatedOn);
                Assert.AreEqual(63, secondComment.Id);
                Assert.AreEqual(null, secondComment.Inline);
                Assert.AreEqual("05/14/2017 15:52:59", secondComment.UpdatedOn);
                Assert.AreEqual("MistyKuuuuuuuu", secondComment.User.DisplayName);
                Assert.AreEqual(62, secondComment.Parent.Id);
            });
        }

        [Test]
        public async Task GetPullRequestCommits_ShouldCallCorrectUrlAndGetResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetEnterpriseDataPath("GetPullRequestCommitsResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<EnterpriseIteratorBasedPage<EnterpriseCommit>>(responseJson);

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<EnterpriseCommit>>((s, url, limit, queryString) => s.GetAllPages<EnterpriseCommit>(url, limit, queryString), responseData.Values);

            var authorResponseJson = Utilities.LoadFile(Paths.GetEnterpriseDataPath("GetAuthorsResponse.json"));
            var authorResponseData = new NewtonsoftJsonSerializer().Deserialize<EnterpriseIteratorBasedPage<EnterpriseUser>>(authorResponseJson);

            var authorResult = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<EnterpriseUser>>((s, url, limit, queryString) => s.GetAllPages<EnterpriseUser>(url, limit, queryString), authorResponseData.Values);

            var resultData = (await _sut.GetPullRequestCommits("reponame", "owner", 1)).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("projects/owner/repos/reponame/pull-requests/1/commits", args.arg1);
                Assert.AreEqual(50, args.arg2);
                Assert.IsNull(args.arg3);

                Assert.AreEqual(responseData.Values.Count, resultData.Count);

                var firstCommit = resultData.First();

                Assert.AreEqual("http://localhost:7990/users/mistyk/avatar.png", firstCommit.Author.User.Links.Avatar.Href);
                Assert.AreEqual("http://url.com/projects/owner/repos/reponame/pull-requests/1/commits/d4a874473e9b6b3e32b4c20f3eea0bf5849876e1", firstCommit.CommitHref);
                Assert.AreEqual("d4a874473e9b6b3e32b4c20f3eea0bf5849876e1", firstCommit.Hash);
                Assert.AreEqual("02/25/2017 10:35:55", firstCommit.Date);
                Assert.AreEqual("asd", firstCommit.Message);
            });

        }

        [Test]
        public async Task GetPullRequestDiff_ShouldCallCorrectUrlAndResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetEnterpriseDataPath("GetPullRequestDiffResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<EnterpriseDiffResponse>(responseJson);

            var response = MockRepository.GenerateMock<IRestResponse<EnterpriseDiffResponse>>();
            response.Stub(x => x.Data).Return(responseData);

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse<EnterpriseDiffResponse>>(
                    (s, req) => s.ExecuteTaskAsync<EnterpriseDiffResponse>(req), response);

            var resultData = (await _sut.GetPullRequestDiff("repoName", "owner", 1)).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("projects/owner/repos/repoName/pull-requests/1/diff", args.Resource);
                Assert.AreEqual(Method.GET, args.Method);

                Assert.AreEqual(1, resultData.Count());

                var firstDiff = resultData.First();

                Assert.AreEqual(FileChangeType.Add, firstDiff.Type);
                Assert.AreEqual(0, firstDiff.Deletions);
                Assert.AreEqual(2, firstDiff.Additions);
                Assert.AreEqual(null, firstDiff.From);
                Assert.AreEqual("new4.txt", firstDiff.To);
                Assert.AreEqual("new4.txt", firstDiff.DisplayFileName);
                Assert.AreEqual(1, firstDiff.Chunks.Count);

                var firstChunk = firstDiff.Chunks.First();

                Assert.AreEqual("asdkopasdkapskdoasd\r\nasdasd", firstChunk.Text);
                Assert.AreEqual(0, firstChunk.OldLines);
                Assert.AreEqual(0, firstChunk.NewLines);
                Assert.AreEqual(null, firstChunk.Content);
                Assert.AreEqual(2, firstChunk.Changes.Count);

                var firstChange = firstChunk.Changes.First();

                Assert.AreEqual(0, firstChange.Index);
                Assert.AreEqual(null, firstChange.OldIndex);
                Assert.AreEqual(1, firstChange.NewIndex);
                Assert.AreEqual(LineChangeType.Add, firstChange.Type);
            });
        }

        [Test]
        public async Task GetPullRequestForBranches_ShouldCallCorrectUrlAndGetResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetEnterpriseDataPath("GetPullRequestForBranchesResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<EnterpriseIteratorBasedPage<EnterprisePullRequest>>(responseJson);

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<EnterprisePullRequest>>((s, url, limit, queryString) => s.GetAllPages<EnterprisePullRequest>(url, limit, queryString), responseData.Values);

            var resultData = (await _sut.GetPullRequestForBranches("reponame", "owner", "hhhhhhhhhhhhhhhh", "12"));

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("projects/owner/repos/reponame/pull-requests", args.arg1);
                Assert.AreEqual(50, args.arg2);
                Assert.AreEqual("?at=refs/heads/12&state=open", args.arg3.ToString());

                Assert.AreEqual("* asd\r\n* asd", resultData.Description);
                Assert.AreEqual("hhhhhhhhhhhhhhhh", resultData.Title);
                Assert.AreEqual("refs/heads/hhhhhhhhhhhhhhhh", resultData.Source.Branch.Name);
                Assert.AreEqual("refs/heads/12", resultData.Destination.Branch.Name);
            });
        }

        [Test]
        public async Task GetPullRequestsPage_ShouldCallCorrectUrlAndResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetEnterpriseDataPath("GetPullRequestsPageResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<EnterpriseIteratorBasedPage<EnterprisePullRequest>>(responseJson);

            var response = MockRepository.GenerateMock<IRestResponse<EnterpriseIteratorBasedPage<EnterprisePullRequest>>>();
            response.Stub(x => x.Data).Return(responseData);

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse<EnterpriseIteratorBasedPage<EnterprisePullRequest>>>(
                    (s, req) => s.ExecuteTaskAsync<EnterpriseIteratorBasedPage<EnterprisePullRequest>>(req), response);

            var resultData = await _sut.GetPullRequestsPage("repoName", "owner", 2);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("projects/owner/repos/repoName/pull-requests", args.Resource);
                Assert.AreEqual(Method.GET, args.Method);

                var limit = args.Parameters.First(x => x.Name == "limit").Value;
                var start = args.Parameters.First(x => x.Name == "start").Value;

                Assert.AreEqual("50", limit);
                Assert.AreEqual("50", start);

                Assert.AreEqual(5, resultData.Values.Count);
                Assert.AreEqual(0, resultData.Page);
                Assert.AreEqual(5, resultData.Size);
                Assert.AreEqual("0", resultData.Next);
                Assert.AreEqual(null, resultData.PageLen);

                var firstPq = resultData.Values.First();

                Assert.AreEqual("* asd\r\n* asd", firstPq.Description);
                Assert.AreEqual("hhhhhhhhhhhhhhhh", firstPq.Title);
            });

        }

        [Test]
        public async Task GetRepositoryUsers_ShouldCallCorrectUrlAndGetResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetEnterpriseDataPath("GetRepositoryUsersResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<EnterpriseIteratorBasedPage<EnterpriseUser>>(responseJson);

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<EnterpriseUser>>((s, url, limit, queryString) => s.GetAllPages<EnterpriseUser>(url, limit, queryString), responseData.Values);

            var resultData = (await _sut.GetRepositoryUsers("reponame", "owner", "filter")).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual($"users", args.arg1);
                Assert.AreEqual(50, args.arg2);
                Assert.AreEqual("?permission=LICENSED_USER&permission.1=REPO_READ&permission.1.projectKey=owner&permission.1.repositorySlug=reponame&filter=filter", args.arg3.ToString());

                var firstUser = resultData.First();

                Assert.AreEqual("MistyKuuuuuuuu", firstUser.DisplayName);
                Assert.AreEqual("mistyksu@gmail.com", firstUser.Email);
                Assert.AreEqual("MistyK", firstUser.Username);
            });

        }

        [Test]
        public async Task UpdatePullRequest_ShouldCallCorrectUrlAndMethod()
        {
            var pq = new PullRequest()
            {
                Id = 1,
                Title = "master",
                Description = "description",
                Source = new Source() { Branch = new Branch() { Name = "master", IsDefault = false } },
                Destination = new Source() { Branch = new Branch() { Name = "4", IsDefault = false } },
                Reviewers = new List<User>() { new User() { Username = "MistyK" } },
            };

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse>((s, req) => s.ExecuteTaskAsync(req), MockRepository.GenerateMock<IRestResponse>());

            await _sut.UpdatePullRequest(pq, "repo", "owner");

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual($"projects/owner/repos/repo/pull-requests/{pq.Id}", args.Resource);
            Assert.AreEqual(Method.PUT, args.Method);
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

            Assert.AreEqual($"projects/owner/repos/repo/pull-requests/{request.Id}/merge?version={request.Version}", args.Resource);
            Assert.AreEqual(Method.POST, args.Method);
        }

        [Test]
        public void GetPullRequestQueryBuilder_ShouldReturnCorrectQueryParams()
        {
            var builder = _sut.GetPullRequestQueryBuilder()
                .WithState("OPEN")
                .WithOrder(Order.Newest)
                .WithSourceBranch("sourceBranch")
                .WithAuthor("user", null);

            Assert.Multiple(() =>
            {
                var expectedResults = new Dictionary<string, string>()
                {
                    ["state"] = "OPEN",
                    ["order"] = "Newest",
                    ["withAttributes"] = "True",
                    ["withProperties"] = "True",
                    ["direction"] = "OUTGOING",
                    ["at"] = "sourceBranch",
                    ["username.1"] = "user",
                    ["role.1"] = "AUTHOR",
                };

                foreach (var queryParameter in builder.GetQueryParameters().Zip(expectedResults, (x, y) => new { Actual = x, Expected = y }))
                {
                    Assert.AreEqual(queryParameter.Expected.Key, queryParameter.Actual.Key);
                    Assert.AreEqual(queryParameter.Expected.Value, queryParameter.Actual.Value);
                }
            });
        }

        [Test]
        public void GetPullRequestQueryBuilder_SourceAndDestSpecified_ShouldThrow()
        {
            var builder = _sut.GetPullRequestQueryBuilder()
                .WithState("OPEN")
                .WithOrder(Order.Newest)
                .WithSourceBranch("sourceBranch")
                .WithAuthor("user", null);

            Assert.Throws<Exception>(() => builder.WithDestinationBranch("destBranch"));
        }

    }
}
