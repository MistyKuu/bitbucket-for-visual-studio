using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using AutoMapper;
using Bitbucket.REST.API.Integration.Tests.Extensions;
using BitBucket.REST.API.Clients.Enterprise;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Mappings;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.Serializers;
using BitBucket.REST.API.Wrappers;
using Newtonsoft.Json;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RestSharp;
using Rhino.Mocks;

namespace Bitbucket.REST.API.Integration.Tests.Enterprise
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

            Assert.AreEqual(args.Resource, "projects/owner/repos/repoName/pull-requests/1/approve");
            Assert.AreEqual(args.Method, Method.POST);

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

            Assert.AreEqual(args.Resource, "projects/owner/repos/repoName/pull-requests/1/approve");
            Assert.AreEqual(args.Method, Method.DELETE);
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

            Assert.AreEqual(args.Resource, $"projects/owner/repos/reponame/pull-requests");
            Assert.AreEqual(args.Method, Method.POST);

            var body = args.Parameters.First(p => p.Type == ParameterType.RequestBody);
            Assert.AreEqual(body.Value, File.ReadAllText(Paths.GetEnterpriseDataPath("CreatePullRequestRequest.json")));
        }
    }
}
