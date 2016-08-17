using System.Threading.Tasks;
using Bitbucket.REST.API.Integration.Tests.Helpers;
using BitBucket.REST.API;
using BitBucket.REST.API.Exceptions;
using BitBucket.REST.API.Models;
using NUnit.Framework;

namespace Bitbucket.REST.API.Integration.Tests
{
    [TestFixture]
    public class BitbucketInitializerTests
    {
        [Test]
        public void Initialize_WithUsername()
        {
            var credentials = new Credentials(CredentialsHelper.TestsCredentials.Username, CredentialsHelper.TestsCredentials.Password);
            var connection = new Connection(credentials);

            var bitbucketClient = new BitBucket.REST.API.BitbucketClient(connection, connection);
            Assert.AreEqual(CredentialsHelper.TestsCredentials.Username, bitbucketClient.Connection.Credentials.Login);
        }

        [Test]
        public async Task Initialize_WithEmail_ShouldFetchUserNameAndOverrideEmail()
        {
            var credentials = new Credentials(CredentialsHelper.TestsCredentials.Email, CredentialsHelper.TestsCredentials.Password);
            var connection = new Connection(credentials);

            var bitbucketInitializer = new BitbucketClientInitializer(connection);

            var bitbucketClient = await bitbucketInitializer.Initialize();

            Assert.AreEqual(CredentialsHelper.TestsCredentials.Username, bitbucketClient.Connection.Credentials.Login);
        }

     
        [Test]
        public void Initialize_WithWrongCredentials_ShouldThrowUnauthorizedException()
        {
            var credentials = new Credentials(CredentialsHelper.TestsCredentials.Email, "asadaszx");
            var connection = new Connection(credentials);

            var bitbucketInitializer = new BitbucketClientInitializer(connection);
        
            Assert.ThrowsAsync<AuthorizationException>(() => bitbucketInitializer.Initialize());

        }
    }
}