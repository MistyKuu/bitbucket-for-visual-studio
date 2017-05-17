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
using RestSharp;
using Rhino.Mocks;

namespace Bitbucket.REST.API.Tests.Standard
{
    [TestFixture]
    public class RepositoriesClientTests
    {
        private IBitbucketRestClient _restClient;
        private RepositoriesClient _sut;
        private IBitbucketRestClient _versionOneClient;

        [SetUp]
        public void SetUp()
        {
            _restClient = MockRepository.GenerateMock<IBitbucketRestClient>();
            _versionOneClient = MockRepository.GenerateMock<IBitbucketRestClient>();
            var connection = new Connection(
                new Uri("http://url.com"),
                new Uri("http://api.url.com"),
                new Credentials("Login", "Password")
                );

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<EnterpriseToStandardMappingsProfile>();
            });

            _sut = new RepositoriesClient(_restClient, _versionOneClient, connection);
        }

        [Test]
        public async Task CreateRepository_ShouldCallCorrectUrlAndMethod()
        {
            var inputRepository = new Repository()
            {
                IsPrivate = true,
                Name = "Test111"
            };


            var responseJson = Utilities.LoadFile(Paths.GetStandardDataPath("CreateRepositoryResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<Repository>(responseJson);

            var response = MockRepository.GenerateMock<IRestResponse<Repository>>();
            response.Stub(x => x.Data).Return(responseData);

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse<Repository>>((s, req) => s.ExecuteTaskAsync<Repository>(req), response);

            var repository = await _sut.CreateRepository(inputRepository);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("repositories/Login/Test111", args.Resource);
                Assert.AreEqual(Method.POST, args.Method);

                var body = args.Parameters.First(x => x.Type == ParameterType.RequestBody);
                var expectedJsonBody = Utilities.LoadFile(Paths.GetStandardDataPath("CreateRepositoryRequest.json"));

                Assert.AreEqual(expectedJsonBody, body.Value.ToString());

                Assert.AreEqual("terere", repository.Name);
                Assert.AreEqual(true, repository.IsPrivate);
            });
        }

        [Test]
        public async Task GetBranches_ShouldCallCorrectUrlAndGetResult()
        {
            var responseDefaultBranchJson = Utilities.LoadFile(Paths.GetStandardDataPath("GetDefaultBranchResponse.json"));
            var responseDefaultBranchData = new NewtonsoftJsonSerializer().Deserialize<Branch>(responseDefaultBranchJson);
            var response = MockRepository.GenerateMock<IRestResponse<Branch>>();
            response.Stub(x => x.Data).Return(responseDefaultBranchData);

            var responseJson = Utilities.LoadFile(Paths.GetStandardDataPath("GetBranchesResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<IteratorBasedPage<Branch>>(responseJson);

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<Branch>>((s, url, limit, queryString) => s.GetAllPages<Branch>(url, limit, queryString), responseData.Values);

            var defaultBranchResult = _versionOneClient
                .Capture()
                .Args<IRestRequest, IRestResponse<Branch>>((s, req) => s.ExecuteTaskAsync<Branch>(req), response);

            var resultData = (await _sut.GetBranches("reponame", "owner")).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("repositories/owner/reponame/refs/branches", args.arg1);
                Assert.AreEqual(50, args.arg2);
                Assert.IsNull(args.arg3);

                var firstBranch = resultData[0];

                Assert.AreEqual(false, firstBranch.IsDefault);
                Assert.AreEqual("ASD-ASD", firstBranch.Name);
                Assert.AreEqual(null, firstBranch.Target.CommitHref);
                Assert.AreEqual("f2fd0045b8ff7ed824b7cd84ae2c9f0d9d2ec91c", firstBranch.Target.Hash);
            });
        }


        [Test]
        public async Task GetCommitsRange_ShouldCallCorrectUrlAndGetResult()
        {
            var sourceBranch = new Branch() { Name = "sourcebranch", Target = new Commit() { Hash = "firstHash" } };
            var destBranch = new Branch() { Name = "destBranch", Target = new Commit() { Hash = "secondHash" } };

            var responseJson = Utilities.LoadFile(Paths.GetStandardDataPath("GetCommitsRangeResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<IteratorBasedPage<Commit>>(responseJson);

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<Commit>>((s, url, limit, queryString) => s.GetAllPages<Commit>(url, limit, queryString), responseData.Values);

            var resultData = (await _sut.GetCommitsRange("reponame", "owner", sourceBranch, destBranch)).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("repositories/owner/reponame/commits/sourcebranch", args.arg1);
                Assert.AreEqual(50, args.arg2);
                Assert.AreEqual("?exclude=destBranch", args.arg3.ToString());

                var firstCommit = resultData[0];

                Assert.AreEqual("NEWFILE created online with Bitbucket", firstCommit.Message);
                Assert.AreEqual("a35d9f3a6642da77a66674fb77cbb9d4fae3f8c1", firstCommit.Hash);
            });
        }

        [Test]
        public async Task GetUserRepositories_ShouldCallCorrectUrlAndGetResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetStandardDataPath("GetUserRepositoriesResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<List<RepositoryV1>>(responseJson);

            var response = MockRepository.GenerateMock<IRestResponse<List<RepositoryV1>>>();
            response.Stub(x => x.Data).Return(responseData);

            var result = _versionOneClient
                .Capture()
                .Args<IRestRequest, IRestResponse<List<RepositoryV1>>>((s, req) => s.ExecuteTaskAsync<List<RepositoryV1>>(req), response);

            var resultData = (await _sut.GetUserRepositories()).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("user/repositories", args.Resource);
                Assert.AreEqual(Method.GET, args.Method);

                var firstRepo = resultData[0];

                Assert.AreEqual("teamrepo", firstRepo.Name);
                Assert.IsNull(firstRepo.Id);
                Assert.AreEqual("git", firstRepo.Scm);
                Assert.AreEqual("2016-07-30 11:48:44+00:00", firstRepo.CreatedOn);
                Assert.AreEqual("2017-03-25 10:41:23+00:00", firstRepo.UpdatedOn);
                Assert.AreEqual("teamrepo", firstRepo.Name);
                Assert.AreEqual(true, firstRepo.IsPrivate);
                Assert.AreEqual("http://Login@url.com/testujemyext/teamrepo.git", firstRepo.Links.Clone.First().Href);
            });
        }

    }
}
