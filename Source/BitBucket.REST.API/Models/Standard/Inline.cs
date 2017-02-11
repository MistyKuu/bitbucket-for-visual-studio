using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class Inline
    {
        [JsonProperty(PropertyName = "to")]
        public int? To { get; set; }

        [JsonProperty(PropertyName = "from")]
        public int? From { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

    }
}