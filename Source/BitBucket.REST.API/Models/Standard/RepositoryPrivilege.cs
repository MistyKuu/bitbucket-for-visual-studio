using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class RepositoryPrivilege
    {
        [JsonProperty(PropertyName = "repo")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "user")]
        public UserShort User { get; set; }

        [JsonProperty(PropertyName = "repository")]
        public Repository Repository { get; set; }
    }
}