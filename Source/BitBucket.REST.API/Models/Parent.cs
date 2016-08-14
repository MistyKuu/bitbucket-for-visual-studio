using Newtonsoft.Json;

namespace BitBucket.REST.API.Models
{
    public class Parent
    {
        [JsonProperty(PropertyName = "links")]
        public Links Links { get; set; }

        [JsonProperty(PropertyName = "full_name")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }
    }
}