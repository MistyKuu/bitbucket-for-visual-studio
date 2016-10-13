using Newtonsoft.Json;

namespace GitClientVS.Contracts.Models
{
    public class ConnectionData
    {
        private ConnectionData(string userName, string password, string host, GitProviderType gitProvider)
        {
            UserName = userName;
            Password = password;
            IsLoggedIn = true;
            Host = host;
            GitProvider = gitProvider;
        }

        private ConnectionData()
        {
            IsLoggedIn = false;
        }

        public static ConnectionData NotLogged => new ConnectionData();

        public static ConnectionData Create(string userName, string password, string host, GitProviderType gitProvider)
        {
            return new ConnectionData(userName, password, host, gitProvider);
        }

        [JsonProperty]
        public string Host { get; private set; }
        [JsonProperty]
        public bool IsLoggedIn { get; private set; }
        [JsonProperty]
        public string UserName { get; private set; }
        [JsonProperty]
        public string Password { get; private set; }
        [JsonProperty]
        public GitProviderType GitProvider { get; private set; }
    }
}