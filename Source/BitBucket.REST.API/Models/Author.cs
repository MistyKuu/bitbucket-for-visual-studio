using Newtonsoft.Json;

namespace BitBucket.REST.API.Models
{
    public class Author
    {
        [JsonProperty(PropertyName = "raw")]
        public string Raw { get; set; }

        [JsonProperty(PropertyName = "user")]
        public UserShort User { get; set; }
    }
}