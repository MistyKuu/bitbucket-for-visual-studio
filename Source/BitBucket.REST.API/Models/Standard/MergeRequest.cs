using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class MergeRequest
    {
        public long Id { get; set; }

        public string Version { get; set; }

        [JsonProperty(PropertyName = "close_source_branch")]
        public bool? CloseSourceBranch { get; set; }

        [JsonProperty(PropertyName = "merge_strategy")]
        public string MergeStrategy { get; set; } //merge_commit, squash, todo enum

    }
}