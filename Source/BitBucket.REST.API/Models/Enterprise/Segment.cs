using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class Segment
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "lines")]
        public List<Line> Lines { get; set; }

        [JsonProperty(PropertyName = "truncated")]
        public bool Truncated { get; set; }
    }
}