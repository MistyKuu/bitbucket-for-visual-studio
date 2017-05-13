using System;
using BitBucket.REST.API;
using BitBucket.REST.API.Exceptions;
using BitBucket.REST.API.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace Bitbucket.REST.API.Integration.Tests
{
    [TestFixture]
    public class BitbucketClientTests { 
    
        [Test]
        public void BasicAuth_WithoutPassingCredentials_ShouldThrowUnauthorizedException()
        {
            // Use USERNAME, not EMAIL ADDRESS, 
            var credentials = new Credentials("test", "test");
            var connection = new Connection(credentials);

            var bitbucketClient = new BitBucket.REST.API.BitbucketClient(connection);
            Assert.ThrowsAsync<AuthorizationException>(() => bitbucketClient.RepositoriesClient.GetRepositories());
        }
    }
}
