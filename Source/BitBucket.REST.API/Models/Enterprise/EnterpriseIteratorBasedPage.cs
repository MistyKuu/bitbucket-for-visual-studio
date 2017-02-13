using System.Collections.Generic;
using BitBucket.REST.API.Models.Standard;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class EnterpriseIteratorBasedPage<T>
    {
        public EnterpriseIteratorBasedPage()
        {
            IsLastPage = true;
        }

        [JsonProperty(PropertyName = "start")]
        public int Start { get; set; }

        [JsonProperty(PropertyName = "limit")]
        public int? Limit { get; set; }

        [JsonProperty(PropertyName = "isLastPage")]
        public bool? IsLastPage { get; set; }

        [JsonProperty(PropertyName = "values")]
        public List<T> Values { get; set; }

        [JsonProperty(PropertyName = "size")]
        public ulong? Size { get; set; }

        [JsonProperty(PropertyName = "nextPageStart")]
        public ulong? NextPageStart { get; set; }

        [JsonProperty(PropertyName = "filter")]
        public string Filter { get; set; }
    }
}