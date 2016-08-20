using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models
{
    public class Branch 
    {
        [JsonProperty(PropertyName = "target")]
        public Commit Target { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}