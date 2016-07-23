using Newtonsoft.Json;

namespace BitBucket.REST.API.Models
{
    public class UserShort
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "links")]
        public Links Links { get; set; }
    }
}