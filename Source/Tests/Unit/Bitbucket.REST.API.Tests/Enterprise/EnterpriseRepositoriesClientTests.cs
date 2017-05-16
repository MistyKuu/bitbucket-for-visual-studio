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
    public class EnterpriseRepositoriesClientTests
    {
        private IEnterpriseBitbucketRestClient _restClient;
        private EnterpriseRepositoriesClient _sut;

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

            _sut = new EnterpriseRepositoriesClient(_restClient, connection);
        }

        [Test]
        public async Task CreateRepository_ShouldCallCorrectUrlAndMethod()
        {
            var inputRepository = new Repository()
            {
                IsPrivate = true,
                Name = "Test111"
            };

            var responseJson = Utilities.LoadFile(Paths.GetEnterpriseDataPath("CreateRepositoryResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<EnterpriseRepository>(responseJson);

            var response = MockRepository.GenerateMock<IRestResponse<EnterpriseRepository>>();
            response.Stub(x => x.Data).Return(responseData);

            var result = _restClient
                .Capture()
                .Args<IRestRequest, IRestResponse<EnterpriseRepository>>((s, req) => s.ExecuteTaskAsync<EnterpriseRepository>(req), response);

            var repository = await _sut.CreateRepository(inputRepository);

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("users/Login/repos", args.Resource);
                Assert.AreEqual(Method.POST, args.Method);

                var body = args.Parameters.First(x => x.Type == ParameterType.RequestBody);
                var expectedJsonBody = Utilities.LoadFile(Paths.GetEnterpriseDataPath("CreateRepositoryRequest.json"));

                Assert.AreEqual(expectedJsonBody, body.Value.ToString());

                Assert.AreEqual("NEWREPOSITORY", repository.Name);
                Assert.AreEqual(true, repository.IsPrivate);
            });
        }

        [Test]
        public async Task GetBranches_ShouldCallCorrectUrlAndGetResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetEnterpriseDataPath("GetBranchesResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<EnterpriseIteratorBasedPage<EnterpriseBranch>>(responseJson);

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<EnterpriseBranch>>((s, url, limit, queryString) => s.GetAllPages<EnterpriseBranch>(url, limit, queryString), responseData.Values);

            var resultData = (await _sut.GetBranches("reponame", "owner")).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("projects/owner/repos/reponame/branches", args.arg1);
                Assert.AreEqual(50, args.arg2);
                Assert.IsNull(args.arg3);

                var firstBranch = resultData[0];

                Assert.AreEqual(false, firstBranch.IsDefault);
                Assert.AreEqual("asdasdasd", firstBranch.Name);
                Assert.AreEqual(null, firstBranch.Target.CommitHref);
                Assert.AreEqual("a2b217a1a7742a1c4a9784012c50df23da43d6fc", firstBranch.Target.Hash);
            });
        }
        

        [Test]
        public async Task GetCommitsRange_ShouldCallCorrectUrlAndGetResult()
        {
            var sourceBranch = new Branch() { Target = new Commit() { Hash = "firstHash" } };
            var destBranch = new Branch() { Target = new Commit() { Hash = "secondHash" } };

            var responseJson = Utilities.LoadFile(Paths.GetEnterpriseDataPath("GetCommitsRangeResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<EnterpriseIteratorBasedPage<EnterpriseCommit>>(responseJson);

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<EnterpriseCommit>>((s, url, limit, queryString) => s.GetAllPages<EnterpriseCommit>(url, limit, queryString), responseData.Values);

            var resultData = (await _sut.GetCommitsRange("reponame", "owner", sourceBranch, destBranch)).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("projects/owner/repos/reponame/commits", args.arg1);
                Assert.AreEqual(50, args.arg2);
                Assert.AreEqual("?until=firstHash&since=secondHash", args.arg3.ToString());

                var firstCommit = resultData[0];

                Assert.AreEqual("short message", firstCommit.Message);
                Assert.AreEqual("28ca84f2acf43e0f935c27d6d96b24e9c09d5fc4", firstCommit.Hash);
            });
        }

        [Test]
        public async Task GetRecentRepositories_ShouldCallCorrectUrlAndGetResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetEnterpriseDataPath("GetRecentRepositoriesResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<EnterpriseIteratorBasedPage<EnterpriseRepository>>(responseJson);

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<EnterpriseRepository>>((s, url, limit, queryString) => s.GetAllPages<EnterpriseRepository>(url, limit, queryString), responseData.Values);

            var resultData = (await _sut.GetRecentRepositories()).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("profile/recent/repos", args.arg1);
                Assert.AreEqual(50, args.arg2);
                Assert.IsNull(args.arg3);

                var firstRepo = resultData[0];

                Assert.AreEqual("test2", firstRepo.Name);
                Assert.AreEqual(12, firstRepo.Id);
            });
        }

        [Test]
        public async Task GetUserRepositories_ShouldCallCorrectUrlAndGetResult()
        {
            var responseJson = Utilities.LoadFile(Paths.GetEnterpriseDataPath("GetUserRepositoriesResponse.json"));
            var responseData = new NewtonsoftJsonSerializer().Deserialize<EnterpriseIteratorBasedPage<EnterpriseRepository>>(responseJson);

            var result = _restClient
                .Capture()
                .Args<string, int, QueryString, IEnumerable<EnterpriseRepository>>((s, url, limit, queryString) => s.GetAllPages<EnterpriseRepository>(url, limit, queryString), responseData.Values);

            var resultData = (await _sut.GetUserRepositories()).ToList();

            Assert.AreEqual(1, result.CallCount);

            var args = result.Args[0];

            Assert.Multiple(() =>
            {
                Assert.AreEqual("repos", args.arg1);
                Assert.AreEqual(50, args.arg2);
                Assert.IsNull(args.arg3);

                var firstRepo = resultData[0];

                Assert.AreEqual("Abc", firstRepo.Name);
                Assert.AreEqual(55, firstRepo.Id);
            });
        }

    }
}
