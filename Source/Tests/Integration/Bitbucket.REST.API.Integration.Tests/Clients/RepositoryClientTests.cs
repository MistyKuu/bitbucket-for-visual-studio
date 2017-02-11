using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitbucket.REST.API.Integration.Tests.Helpers;
using BitBucket.REST.API;
using BitBucket.REST.API.Clients;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.Serializers;
using BitBucket.REST.API.Wrappers;
using NUnit.Framework;

namespace Bitbucket.REST.API.Integration.Tests.Clients
{
    public class RepositoryClientTests
    {
        private BitbucketClient bitbucketClient;

        [SetUp]
        public void GlobalSetup()
        {
            var credentials = new Credentials(CredentialsHelper.TestsCredentials.Username, CredentialsHelper.TestsCredentials.Password);
            var connection = new Connection(CredentialsHelper.TestsCredentials.HostUrl, CredentialsHelper.TestsCredentials.ApiUrl, credentials);

            bitbucketClient = new BitBucket.REST.API.BitbucketClient(connection, connection);
        }

        [Test]
        public void GetAllRepositories()
        {
            var repositories = bitbucketClient.RepositoriesClient.GetRepositories();
        }
        [Test]
        public async Task GetAllRepositories_Enterprise()
        {
            var credentials = new Credentials(CredentialsHelper.TestsCredentials.Username, CredentialsHelper.TestsCredentials.Password);
            var connection = new Connection(new Uri("http://localhost:7990"), new Uri("http://localhost:7990"), credentials);

            var bitbucketClient = new EnterpriseBitbucketClient(connection, connection);
            var test = await bitbucketClient.RepositoriesClient.GetRepositories();
            var test2 = await bitbucketClient.RepositoriesClient.GetBranches(test.Values[0].Name);
            var test3 = await bitbucketClient.RepositoriesClient.CreateRepository(new Repository()
            {
                Name = "ADS",
                Description = "NOT USED",
                IsPrivate = true
            });
        }
    }
}
