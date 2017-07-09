using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class Inline
    {
        [JsonProperty(PropertyName = "to")]
        public long? To { get; set; }

        [JsonProperty(PropertyName = "from")]
        public long? From { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

    }
}