using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class Branch 
    {
        [JsonProperty(PropertyName = "target")]
        public Commit Target { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        
        public bool? IsDefault { get; set; }
    }
}