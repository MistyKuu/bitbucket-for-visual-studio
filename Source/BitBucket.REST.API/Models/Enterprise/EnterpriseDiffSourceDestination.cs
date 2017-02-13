using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class EnterpriseDiffSourceDestination
    {
        [JsonProperty(PropertyName = "components")]
        public List<string> Components { get; set; }

        [JsonProperty(PropertyName = "parent")]
        public string Parent { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "extension")]
        public string Extension { get; set; }

        [JsonProperty(PropertyName = "toString")]
        public string String { get; set; }
    }
}