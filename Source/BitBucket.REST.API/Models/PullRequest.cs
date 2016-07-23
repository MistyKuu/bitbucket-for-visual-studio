using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models
{
    public class PullRequest
    {
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "links")]
        public Links Links { get; set; }

        [JsonProperty(PropertyName = "author")]
        public User Author { get; set; }

        [JsonProperty(PropertyName = "close_source_branch")]
        public bool? CloseSourceBranch { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "destination")]
        public Source Destination { get; set; }

        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }

        [JsonProperty(PropertyName = "closed_by")]
        public object ClosedBy { get; set; }

        [JsonProperty(PropertyName = "source")]
        public Source Source { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string State { get; set; }

        [JsonProperty(PropertyName = "created_on")]
        public string CreatedOn { get; set; }

        [JsonProperty(PropertyName = "updated_on")]
        public string UpdatedOn { get; set; }

        [JsonProperty(PropertyName = "merge_commit")]
        public object MergeCommit { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int? Id { get; set; }
    }
}
