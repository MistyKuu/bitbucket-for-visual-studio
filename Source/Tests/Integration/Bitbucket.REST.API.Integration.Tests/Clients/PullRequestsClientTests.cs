using System.Threading.Tasks;
using Bitbucket.REST.API.Integration.Tests.Helpers;
using BitBucket.REST.API;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using NUnit.Framework;

namespace Bitbucket.REST.API.Integration.Tests.Clients
{
    public class PullRequestsClientTests
    {
        private BitbucketClient bitbucketClient;

        [SetUp]
        public void GlobalSetup()
        {
            var credentials = new Credentials(CredentialsHelper.TestsCredentials.Username, CredentialsHelper.TestsCredentials.Password);
            var connection = new Connection(CredentialsHelper.TestsCredentials.HostUrl,CredentialsHelper.TestsCredentials.ApiUrl, credentials);

            bitbucketClient = new BitBucket.REST.API.BitbucketClient(connection, connection, connection);
        }

        [Test]
        public async Task GetAllRepositories()
        {
            //var pullRequests = await bitbucketClient.PullRequestsClient.Ge("test");

            //var testStatus = await bitbucketClient.PullRequestsClient.GetPullRequests("test", PullRequestOptions.MERGED);

            //var newPullRequest = new PullRequest();
            //newPullRequest.Title = "testRest";
            //newPullRequest.Description = "testRestDesc";
            //newPullRequest.Source = new Source
            //{
            //    Branch = new Branch()
            //    {
            //        Name = "testbranch"
            //    },
            //    Repository = new Repository()
            //    {
            //        Name = "test"
            //    }
            //};
            //newPullRequest.Destination = new Source()
            //{
            //    Branch = new Branch()
            //    {
            //        Name = "master"
            //    }
            //};


            //var result = await bitbucketClient.PullRequestsClient.CreatePullRequest(newPullRequest, "test");

        }
    }
}