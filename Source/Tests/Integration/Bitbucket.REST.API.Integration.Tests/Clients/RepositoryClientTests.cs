using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitbucket.REST.API.Integration.Tests.Helpers;
using BitBucket.REST.API;
using BitBucket.REST.API.Models;
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
            var connection = new Connection(credentials);

            bitbucketClient = new BitBucket.REST.API.BitbucketClient(connection, connection);
        }

        [Test]
        public void GetAllRepositories()
        {
            var repositories = bitbucketClient.RepositoriesClient.GetRepositories();
        }

    }
}
