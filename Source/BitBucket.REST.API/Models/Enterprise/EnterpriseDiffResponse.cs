using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class EnterpriseDiffResponse
    {
        [JsonProperty(PropertyName = "fromHash")]
        public string FromHash { get; set; }

        [JsonProperty(PropertyName = "toHash")]
        public string ToHash { get; set; }

        [JsonProperty(PropertyName = "contextLines")]
        public int ContextLines { get; set; }

        [JsonProperty(PropertyName = "whitespace")]
        public string Whitespace { get; set; }

        [JsonProperty(PropertyName = "diffs")]
        public List<EnterpriseDiff> Diffs { get; set; }

        [JsonProperty(PropertyName = "truncated")]
        public bool Truncated { get; set; }

        [JsonProperty(PropertyName = "binary")]
        public bool IsBinary { get; set; }
    }
}
