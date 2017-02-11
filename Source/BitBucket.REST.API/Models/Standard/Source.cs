using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class Source
    {
        [JsonProperty(PropertyName = "commit")]
        public Commit Commit { get; set; }

        [JsonProperty(PropertyName = "repository")]
        public Repository Repository { get; set; }

        [JsonProperty(PropertyName = "branch")]
        public Branch Branch { get; set; }
    }
}