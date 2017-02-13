using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class EnterpriseDiff
    {
        [JsonProperty(PropertyName = "source")]
        public EnterpriseDiffSourceDestination Source { get; set; }

        [JsonProperty(PropertyName = "destination")]
        public EnterpriseDiffSourceDestination Destination { get; set; }

        [JsonProperty(PropertyName = "hunks")]
        public List<Hunk> Hunks { get; set; }

        [JsonProperty(PropertyName = "truncated")]
        public bool Truncated { get; set; }
    }
}