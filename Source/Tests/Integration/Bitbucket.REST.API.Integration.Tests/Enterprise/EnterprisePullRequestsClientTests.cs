using System;
using BitBucket.REST.API.Clients.Enterprise;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;
using NUnit.Framework;
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

            _sut = new EnterprisePullRequestsClient(_restClient, connection);
        }

        [SetUp]
        public void Do()
        {
           // var url = $"projects/owner/repos/repoName/pull-requests/id/approve";
           //_restClient.Expect(x=>x.ExecuteTaskAsync<EnterpriseParticipant>())

           // var response = _sut.ApprovePullRequest("asd", "asd", 12);
        }
        //var url = EnterpriseApiUrls.PullRequestApprove(ownerName, repositoryName, id);
        //var request = new BitbucketRestRequest(url, Method.POST);
        //var response = await RestClient.ExecuteTaskAsync<EnterpriseParticipant>(request);
        //    return response.Data.MapTo<Participant>();
    }
}
