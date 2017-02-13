using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class Line
    {
        [JsonProperty(PropertyName = "source")]
        public int Source { get; set; }

        [JsonProperty(PropertyName = "destination")]
        public int Destination { get; set; }

        [JsonProperty(PropertyName = "line")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "truncated")]
        public bool Truncated { get; set; }
    }
}