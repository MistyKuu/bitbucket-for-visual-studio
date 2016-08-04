using Newtonsoft.Json;

namespace BitBucket.REST.API.Models
{
    public class Content
    {
        [JsonProperty(PropertyName = "raw")]
        public string Raw { get; set; }

        [JsonProperty(PropertyName = "markup")]
        public string Markup { get; set; }

        [JsonProperty(PropertyName = "html")]
        public string Html { get; set; }
    }
}