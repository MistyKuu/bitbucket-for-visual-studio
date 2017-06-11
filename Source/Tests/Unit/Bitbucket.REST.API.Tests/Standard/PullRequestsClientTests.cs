using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BitBucket.REST.API.Clients.Enterprise;
using BitBucket.REST.API.Clients.Standard;
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

namespace Bitbucket.REST.API.Tests.Standard
{
    [TestFixture]
    public class PullRequestsClientTests
    {
        private IBitbucketRestClient _restClient;
        private PullRequestsClient _sut;
        private IBitbucketRestClient _internalClient;
        private IBitbucketRestClient _webClient;
        private IBitbucketRestClient _versionOneClient;

        [SetUp]
        public void SetUp()
        {
            _restClient = MockRepository.GenerateMock<IBitbucketRestClient>();
            _internalClient = MockRepository.GenerateMock<IBitbucketRestClient>();
            _webClient = MockRepository.GenerateMock<IBitbucketRestClient>();
            _versionOneClient = MockRepository.GenerateMock<IBitbucketRestClient>();

            var connection = new Connection(
                new Uri("http://url.com"),
                new Uri("http://api.url.com"),
                new Credentials("mistyku", "Password")
                );

            _sut = new PullRequestsClient(_restClient, _internalClient, _webClient, _versionOneClient, connection);
        }

        [Test]
        public async Task ApprovePullRequest_ShouldReturnValidParticipant()
        {
            var responseJson = Utilities.LoadFile(Paths.GetStandardDataPath("ApprovePullRequestResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<Participant>(responseJson);

            var response = MockRepository.GenerateMock<IRestResponse<Participant>>();
            response.Stub(x => x.Data).Return(responseData);

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse<Participant>>((s, req) => s.ExecuteTaskAsync<Participant>(req), response);

            var participant = await _sut.ApprovePullRequest("repoName", "owner", 1);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("repositories/owner/repoName/pullrequests/1/approve", args.Resource);
                Assert.AreEqual(Method.POST, args.Method);

                Assert.AreEqual(true, participant.Approved);
                Assert.AreEqual("PARTICIPANT", participant.Role);
                Assert.AreEqual("mistyku", participant.User.Username);
                Assert.AreEqual("Zibi", participant.User.DisplayName);
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

            Assert.AreEqual("repositories/owner/repoName/pullrequests/1/approve", args.Resource);
            Assert.AreEqual(Method.DELETE, args.Method);
        }

        [Test]
        public async Task CreatePullRequest_ShouldCallCorrectUrlAndMethod()
        {
            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse<PullRequest>>((s, req) => s.ExecuteTaskAsync<PullRequest>(req), MockRepository.GenerateMock<IRestResponse<PullRequest>>());

            var pq = new PullRequest()
            {
                Id = 0,
                Title = "master",
                Description = "* qwdwqdqwd created online with Bitbucket",
                Source = new Source() { Branch = new Branch() { Name = "master", IsDefault = false } },
                Destination = new Source() { Branch = new Branch() { Name = "testbranch", IsDefault = false } },
                Reviewers = new List<User>() { new User() { Username = "bitbucketvsextension", Type = "user" } },
                CloseSourceBranch = false,
                State = PullRequestOptions.OPEN
            };

            await _sut.CreatePullRequest(pq, "reponame", "owner");

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual("repositories/owner/reponame/pullrequests", args.Resource);
            Assert.AreEqual(Method.POST, args.Method);

            var body = args.Parameters.First(p => p.Type == ParameterType.RequestBody);

            var expectedJsonBody = Utilities.LoadFile(Paths.GetStandardDataPath("CreatePullRequestRequest.json"));

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

            Assert.AreEqual("repositories/owner/reponame/pullrequests/1/decline", args.Resource);
            Assert.AreEqual(Method.POST, args.Method);
        }

        [Test]
        public async Task GetAuthors_ShouldCallCorrectUrlAndResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetStandardDataPath("GetAuthorsResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<IteratorBasedPage<UserShort>>(responseJson);

            var result = _internalClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<UserShort>>((s, url, limit, queryString) => s.GetAllPages<UserShort>(url, limit, queryString), responseData.Values);

            var resultAuthors = (await _sut.GetAuthors("reponame", "owner")).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual("repositories/owner/reponame/pr-authors/", args.arg1);
            Assert.AreEqual(100, args.arg2);
            Assert.IsNull(args.arg3);

            var firstResultAuthor = resultAuthors.First();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(responseData.Values.Count, resultAuthors.Count);
                Assert.AreEqual("bitbucketvsextension", firstResultAuthor.DisplayName);
                Assert.AreEqual("bitbucketvsextension", firstResultAuthor.Username);
                Assert.AreEqual("https://bitbucket.org/!api/2.0/users/bitbucketvsextension", firstResultAuthor.Links.Self.Href);

            });
        }

        [Test]
        public async Task GetCommitsDiff_ShouldCallCorrectUrlAndResult()
        {
            var responseTxt = Utilities.LoadFile(Paths.GetStandardDataPath("GetCommitsDiffResponse.txt"));
            responseTxt = responseTxt.Replace(Environment.NewLine, "\n");

            var response = MockRepository.GenerateMock<IRestResponse>();
            response.Stub(x => x.Content).Return(responseTxt);

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse>(
                    (s, req) => s.ExecuteTaskAsync(req), response);

            var commitDiff = await _sut.GetCommitsDiff("repoName", "owner", "9f0a336dc13893f8a73693267662a1b5835a3d87", "9f0a336dc13893f8a73693267662a1b5835a3d84");

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];
            Assert.Multiple(() =>
            {
                Assert.AreEqual("repositories/owner/repoName/diff/9f0a336dc13893f8a73693267662a1b5835a3d87..9f0a336dc13893f8a73693267662a1b5835a3d84", args.Resource);
                Assert.AreEqual(Method.GET, args.Method);

                var firstDiff = commitDiff.First();

                Assert.AreEqual(FileChangeType.Add, firstDiff.Type);
                Assert.AreEqual(0, firstDiff.Deletions);
                Assert.AreEqual(1, firstDiff.Additions);
                Assert.AreEqual("/dev/null", firstDiff.From);
                Assert.AreEqual("NEWFILE", firstDiff.To);
                Assert.AreEqual("NEWFILE", firstDiff.DisplayFileName);
                Assert.AreEqual(1, firstDiff.Chunks.Count);

                var firstChange = firstDiff.Chunks.First().Changes.First();

                Assert.AreEqual("+wqdqwdwqd", firstChange.Content);
                Assert.AreEqual(1,firstChange.Index);
                Assert.AreEqual(1,firstChange.NewIndex);
                Assert.AreEqual(null,firstChange.OldIndex);
                Assert.AreEqual(LineChangeType.Add,firstChange.Type);

            });
        }

        [Test]
        public async Task GetDefaultReviewers_ShouldCallCorrectUrlAndResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetStandardDataPath("GetDefaultReviewersResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<IteratorBasedPage<UserShort>>(responseJson);

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<UserShort>>((s, url, limit, queryString) => s.GetAllPages<UserShort>(url, limit, queryString), responseData.Values);

            var resultData = (await _sut.GetDefaultReviewers("reponame", "owner")).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("repositories/owner/reponame/default-reviewers", args.arg1);
                Assert.AreEqual(100, args.arg2);
                Assert.IsNull(args.arg3);

                var firstReviewer = resultData[0];

                Assert.AreEqual("bitbucketvsextension", firstReviewer.DisplayName);
            });
        }

        [Test]
        public async Task GetPullRequest_ShouldReturnValidResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetStandardDataPath("GetPullRequestResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<PullRequest>(responseJson);

            var response = MockRepository.GenerateMock<IRestResponse<PullRequest>>();
            response.Stub(x => x.Data).Return(responseData);

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse<PullRequest>>((s, req) => s.ExecuteTaskAsync<PullRequest>(req), response);

            var resultPullRequest = await _sut.GetPullRequest("repoName", "owner", 1);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("repositories/owner/repoName/pullrequests/1", args.Resource);
                Assert.AreEqual(Method.GET, args.Method);

                Assert.AreEqual("2017-04-07T20:21:29.723251+00:00", resultPullRequest.CreatedOn);
                Assert.AreEqual("2017-04-24T21:03:21.455916+00:00", resultPullRequest.UpdatedOn);
                Assert.AreEqual(false, resultPullRequest.CloseSourceBranch);
                Assert.AreEqual("short description", resultPullRequest.Description);
                Assert.AreEqual("testbranch1122", resultPullRequest.Title);
                Assert.AreEqual(21, resultPullRequest.Id);
                Assert.AreEqual(PullRequestOptions.OPEN, resultPullRequest.State);
                Assert.AreEqual("testbranch", resultPullRequest.Source.Branch.Name);
                Assert.AreEqual("ASD-ASD", resultPullRequest.Destination.Branch.Name);
            });
        }

        [Test]
        public async Task GetPullRequestComments_ShouldCallCorrectUrlAndGetResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetStandardDataPath("GetPullRequestCommentsResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<IteratorBasedPage<Comment>>(responseJson);

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<Comment>>((s, url, limit, queryString) => s.GetAllPages<Comment>(url, limit, queryString), responseData.Values);

            var resultData = (await _sut.GetPullRequestComments("reponame", "owner", 1)).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("repositories/owner/reponame/pullrequests/1/comments", args.arg1);
                Assert.AreEqual(50, args.arg2);
                Assert.IsNull(args.arg3);

                var secondComment = resultData[1];

                Assert.AreEqual("<p>Response to hello!</p>", secondComment.Content.Html);
                Assert.AreEqual("Response to hello!", secondComment.Content.Raw);
                Assert.AreEqual("2017-05-14T17:09:00.032164+00:00", secondComment.CreatedOn);
                Assert.AreEqual(36823272, secondComment.Id);
                Assert.AreEqual(null, secondComment.Inline);
                Assert.AreEqual("2017-05-14T17:09:00.034473+00:00", secondComment.UpdatedOn);
                Assert.AreEqual("dispname", secondComment.User.DisplayName);
                Assert.AreEqual(36823271, secondComment.Parent.Id);
            });
        }

        [Test]
        public async Task GetPullRequestCommits_ShouldCallCorrectUrlAndGetResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetStandardDataPath("GetPullRequestCommitsResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<IteratorBasedPage<Commit>>(responseJson);

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<Commit>>((s, url, limit, queryString) => s.GetAllPages<Commit>(url, limit, queryString), responseData.Values);

            var resultData = (await _sut.GetPullRequestCommits("reponame", "owner", 1)).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("repositories/owner/reponame/pullrequests/1/commits", args.arg1);
                Assert.AreEqual(50, args.arg2);
                Assert.IsNull(args.arg3);

                Assert.AreEqual(responseData.Values.Count, resultData.Count);

                var firstCommit = resultData.First();

                Assert.AreEqual("https://bitbucket.org/account/mistyku/avatar/32/", firstCommit.Author.User.Links.Avatar.Href);
                Assert.AreEqual("http://url.com/owner/reponame/commits/b6988a77079d7f96127c085b5c34077d24e4f096", firstCommit.CommitHref);
                Assert.AreEqual("b6988a77079d7f96127c085b5c34077d24e4f096", firstCommit.Hash);
                Assert.AreEqual("2017-04-07T20:10:54+00:00", firstCommit.Date);
                Assert.AreEqual("short message", firstCommit.Message);
            });

        }

        [Test]
        public async Task GetPullRequestDiff_ShouldCallCorrectUrlAndResult()
        {
            var responseTxt = Utilities.LoadFile(Paths.GetStandardDataPath("GetPullRequestDiffResponse.txt"));
            responseTxt = responseTxt.Replace(Environment.NewLine, "\n");

            var response = MockRepository.GenerateMock<IRestResponse>();
            response.Stub(x => x.Content).Return(responseTxt);

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse>(
                    (s, req) => s.ExecuteTaskAsync(req), response);

            var resultData = (await _sut.GetPullRequestDiff("repoName", "owner", 1)).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("repositories/owner/repoName/pullrequests/1/diff", args.Resource);
                Assert.AreEqual(Method.GET, args.Method);

                Assert.AreEqual(1, resultData.Count());

                var firstDiff = resultData.First();

                Assert.AreEqual(FileChangeType.Add, firstDiff.Type);
                Assert.AreEqual(0, firstDiff.Deletions);
                Assert.AreEqual(1, firstDiff.Additions);
                Assert.AreEqual("/dev/null", firstDiff.From);
                Assert.AreEqual("NEWFILE", firstDiff.To);
                Assert.AreEqual("NEWFILE", firstDiff.DisplayFileName);
                Assert.AreEqual(1, firstDiff.Chunks.Count);

                var firstChunk = firstDiff.Chunks.First();

                Assert.AreEqual("+wqdqwdwqd", firstChunk.Text);
                Assert.AreEqual(0, firstChunk.OldLines);
                Assert.AreEqual(0, firstChunk.NewLines);
                Assert.AreEqual("@@ -0,0 +1 @@", firstChunk.Content);
                Assert.AreEqual(1, firstChunk.Changes.Count);

                var firstChange = firstChunk.Changes.First();

                Assert.AreEqual(1, firstChange.Index);
                Assert.AreEqual(null, firstChange.OldIndex);
                Assert.AreEqual(1, firstChange.NewIndex);
                Assert.AreEqual(LineChangeType.Add, firstChange.Type);
            });
        }

        [Test]
        public async Task GetPullRequestForBranches_ShouldCallCorrectUrlAndGetResult()
        {
            var pullRequestResponseJson = Utilities.LoadFile(Paths.GetStandardDataPath("GetPullRequestForBranchesWithReviewersResponse.json"));
            var pullRequestResponseData = new NewtonsoftJsonSerializer().Deserialize<PullRequest>(pullRequestResponseJson);
            var response = MockRepository.GenerateMock<IRestResponse<PullRequest>>();
            response.Stub(x => x.Data).Return(pullRequestResponseData);

            var responseJson = Utilities.LoadFile(Paths.GetStandardDataPath("GetPullRequestForBranchesResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<IteratorBasedPage<PullRequest>>(responseJson);

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<PullRequest>>((s, url, limit, queryString) => s.GetAllPages<PullRequest>(url, limit, queryString), responseData.Values);

            var pullRequestResult = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse<PullRequest>>(
                    (s, req) => s.ExecuteTaskAsync<PullRequest>(req), response);

            var resultData = (await _sut.GetPullRequestForBranches("reponame", "owner", "hhhhhhhhhhhhhhhh", "12"));

            Assert.AreEqual(1, pullRequestResult.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("repositories/owner/reponame/pullrequests", args.arg1);
                Assert.AreEqual(20, args.arg2);
                Assert.AreEqual(@"?q=source.branch.name = ""hhhhhhhhhhhhhhhh"" AND destination.branch.name = ""12"" AND state = ""OPEN""", args.arg3.ToString());

                Assert.AreEqual("* NEWFILE created online with Bitbucket", resultData.Description);
                Assert.AreEqual("master", resultData.Title);
                Assert.AreEqual("master", resultData.Source.Branch.Name);
                Assert.AreEqual("testbranch", resultData.Destination.Branch.Name);
                Assert.AreEqual("bitbucketvsextension", resultData.Reviewers.First().Username);
            });
        }

        [Test]
        public async Task GetPullRequestsPage_ShouldCallCorrectUrlAndResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetStandardDataPath("GetPullRequestsPageResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<IteratorBasedPage<PullRequest>>(responseJson);

            var response = MockRepository.GenerateMock<IRestResponse<IteratorBasedPage<PullRequest>>>();
            response.Stub(x => x.Data).Return(responseData);

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse<IteratorBasedPage<PullRequest>>>(
                    (s, req) => s.ExecuteTaskAsync<IteratorBasedPage<PullRequest>>(req), response);

            var resultData = await _sut.GetPullRequestsPage("repoName", "owner", 2);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("repositories/owner/repoName/pullrequests", args.Resource);
                Assert.AreEqual(Method.GET, args.Method);

                var pageLen = args.Parameters.First(x => x.Name == "pagelen").Value;
                var page = args.Parameters.First(x => x.Name == "page").Value;

                Assert.AreEqual("50", pageLen);
                Assert.AreEqual("2", page);

                Assert.AreEqual(3, resultData.Values.Count);
                Assert.AreEqual(1, resultData.Page);
                Assert.AreEqual(3, resultData.Size);
                Assert.AreEqual(null, resultData.Next);
                Assert.AreEqual(50, resultData.PageLen);

                var firstPq = resultData.Values.First();

                Assert.AreEqual("short description", firstPq.Description);
                Assert.AreEqual("testbranch1122", firstPq.Title);
            });

        }

        [Test]
        public async Task GetRepositoryUsers_ShouldCallCorrectUrlAndGetResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetStandardDataPath("GetRepositoryUsersResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<List<UserShort>>(responseJson);

            var response = MockRepository.GenerateMock<IRestResponse<List<UserShort>>>();
            response.Stub(x => x.Data).Return(responseData);

            var result = _webClient
                .Capture()
                .Args<IRestRequest, IRestResponse<List<UserShort>>>(
                    (s, req) => s.ExecuteTaskAsync<List<UserShort>>(req), response);

            var resultData = (await _sut.GetRepositoryUsers("reponame", "owner", "filter")).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("mentions/repositories/owner/reponame", args.Resource);
                Assert.AreEqual(Method.GET, args.Method);


                var firstUser = resultData.First();

                Assert.AreEqual("Zibi", firstUser.DisplayName);
                Assert.AreEqual(null, firstUser.Email);
                Assert.AreEqual("mistyku", firstUser.Username);
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
                .Args<IRestRequest, IRestResponse<PullRequest>>((s, req) => s.ExecuteTaskAsync<PullRequest>(req), MockRepository.GenerateMock<IRestResponse<PullRequest>>());

            await _sut.UpdatePullRequest(pq, "repo", "owner");

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.AreEqual("repositories/owner/repo/pullrequests/1", args.Resource);
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

            Assert.AreEqual($"repositories/owner/repo/pullrequests/{request.Id}/merge", args.Resource);
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
                    ["sort"] = "-updated_on",
                    ["q"] = @"source.branch.name = ""sourceBranch"" AND author.username = ""user""",
                };

                foreach (var queryParameter in builder.GetQueryParameters().Zip(expectedResults, (x, y) => new { Actual = x, Expected = y }))
                {
                    Assert.AreEqual(queryParameter.Expected.Key, queryParameter.Actual.Key);
                    Assert.AreEqual(queryParameter.Expected.Value, queryParameter.Actual.Value);
                }
            });
        }
    }
}
