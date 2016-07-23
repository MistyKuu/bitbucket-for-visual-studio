using Newtonsoft.Json;

namespace BitBucket.REST.API.Models
{
    public class Branch
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}