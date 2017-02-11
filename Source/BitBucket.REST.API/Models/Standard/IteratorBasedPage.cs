using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class IteratorBasedPage<T>
    {
        [JsonProperty(PropertyName = "pagelen")]
        public int? PageLen { get; set; }

        [JsonProperty(PropertyName = "page")]
        public int Page { get; set; }

        [JsonProperty(PropertyName = "next")]
        public string Next { get; set; }

        [JsonProperty(PropertyName = "values")]
        public List<T> Values { get; set; }

        [JsonProperty(PropertyName = "size")]
        public ulong? Size { get; set; }
    }
}