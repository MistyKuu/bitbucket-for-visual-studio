using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class PermissionDto
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "user")]
        public User User { get; set; }
        [JsonProperty(PropertyName = "permission")]
        public string Permission { get; set; }
    }
}