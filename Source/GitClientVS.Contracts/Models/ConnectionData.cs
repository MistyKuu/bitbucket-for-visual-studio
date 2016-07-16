using Newtonsoft.Json;

namespace GitClientVS.Contracts.Models
{
    public class ConnectionData
    {
        private ConnectionData(string userName, string password)
        {
            UserName = userName;
            Password = password;
            IsLoggedIn = true;
        }

        private ConnectionData()
        {
            IsLoggedIn = false;
        }

        public static ConnectionData NotLogged => new ConnectionData();

        public static ConnectionData Create(string userName, string password)
        {
            return new ConnectionData(userName, password);
        }

        [JsonProperty]
        public bool IsLoggedIn { get; private set; }
        [JsonProperty]
        public string UserName { get; private set; }
        [JsonProperty]
        public string Password { get; private set; }
    }
}