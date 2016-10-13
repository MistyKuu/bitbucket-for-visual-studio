using Newtonsoft.Json;

namespace GitClientVS.Contracts.Models
{
    public class ConnectionData
    {
        public static ConnectionData NotLogged => new ConnectionData() {IsLoggedIn = false};

        [JsonProperty]
        public bool IsLoggedIn { get; set; }
        [JsonProperty]
        public string UserName { get; set; }
        [JsonProperty]
        public string Password { get; set; }
        [JsonProperty]
        public string Host { get; set; }
    }
}