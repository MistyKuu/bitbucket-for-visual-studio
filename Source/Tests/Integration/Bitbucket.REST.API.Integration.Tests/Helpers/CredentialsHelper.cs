using System;
using System.IO;
using BitBucket.REST.API.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Bitbucket.REST.API.Integration.Tests.Helpers
{
    public static class CredentialsHelper
    {
        static readonly Lazy<FakeCredentials> _credentialsThunk = new Lazy<FakeCredentials>(() =>
        {
            JsonSerializer serializer = new JsonSerializer();
            using (JsonTextReader reader = new JsonTextReader(File.OpenText(TestContext.CurrentContext.TestDirectory + @"\credentials.json")))
            {
                FakeCredentials testsCredentials = serializer.Deserialize<FakeCredentials>(reader);
                return testsCredentials;
            }
        });

        public static FakeCredentials TestsCredentials { get { return _credentialsThunk.Value; } }
        
    }

    public class FakeCredentials
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Uri Host { get; set; } = new Uri("https://bitbucket.org");
    }
}