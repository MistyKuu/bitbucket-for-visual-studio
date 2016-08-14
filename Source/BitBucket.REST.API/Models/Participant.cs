using Newtonsoft.Json;

namespace BitBucket.REST.API.Models
{
    public class Participant
    {
        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; }

        [JsonProperty(PropertyName = "approved")]
        public bool Approved { get; set; }

        [JsonProperty(PropertyName = "user")]
        public UserShort User { get; set; }
    }
}