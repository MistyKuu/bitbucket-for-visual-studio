using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class User
    {
        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "uuid")]
        public string Uuid { get; set; }

        [JsonProperty(PropertyName = "links")]
        public Links Links { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }
    }
}