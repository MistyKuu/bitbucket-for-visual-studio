using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class EnterpriseBrowsePage
    {
        [JsonProperty(PropertyName = "lines")]
        public List<EnterpriseBrowseText> Lines { get; set; }

        [JsonProperty(PropertyName = "start")]
        public int Start { get; set; }

        [JsonProperty(PropertyName = "limit")]
        public int? Limit { get; set; }

        [JsonProperty(PropertyName = "isLastPage")]
        public bool? IsLastPage { get; set; }

        [JsonProperty(PropertyName = "size")]
        public ulong? Size { get; set; }
    }
}
