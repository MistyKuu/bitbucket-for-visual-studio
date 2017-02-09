using Bitbucket.REST.API.Integration.Tests.Helpers;
using BitBucket.REST.API.Exceptions;
using BitBucket.REST.API.Models;
using NUnit.Framework;

namespace Bitbucket.REST.API.Integration.Tests.Clients
{
    [TestFixture]
    public class UserClientTests
    {

        [Test]
        public void BasicAuth_WithoutPassingCredentials_ShouldThrowUnauthorizedException()
        {
            // Use USERNAME, not EMAIL ADDRESS, 
            var credentials = new Credentials("test", "test");
            var connection = new Connection(CredentialsHelper.TestsCredentials.Host, credentials);

            var bitbucketClient = new BitBucket.REST.API.BitbucketClient(connection, connection);

            Assert.ThrowsAsync<AuthorizationException>(() => bitbucketClient.UserClient.GetUser());
        }

        [Test]
        public void BasicAuth_WithoutPassingCredentials_ShouldThrowUnauthorizedException2()
        {
            // Use USERNAME, not EMAIL ADDRESS, 
            var credentials = new Credentials("test", "test");
            var connection = new Connection(CredentialsHelper.TestsCredentials.Host,credentials);

            var bitbucketClient = new BitBucket.REST.API.BitbucketClient(connection, connection);

            Assert.ThrowsAsync<AuthorizationException>(() => bitbucketClient.UserClient.GetUser());
        }
    }
}