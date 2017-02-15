using System.Collections.Generic;
using BitBucket.REST.API.Models.Standard;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class EnterprisePullRequest
    {
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "links")]
        public EnterpriseLinks Links { get; set; }

        [JsonProperty(PropertyName = "author")]
        public EnterpriseParticipant Author { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "toRef")]
        public EnterpriseBranchSource Destination { get; set; }

        [JsonProperty(PropertyName = "fromRef")]
        public EnterpriseBranchSource Source { get; set; }

        [JsonProperty(PropertyName = "state")]
        public EnterprisePullRequestOptions State { get; set; }

        [JsonProperty(PropertyName = "createdDate")]
        public long CreatedOn { get; set; }

        [JsonProperty(PropertyName = "updatedDate")]
        public long UpdatedOn { get; set; }

        [JsonProperty(PropertyName = "merge_commit")]
        public object MergeCommit { get; set; }

        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public EnterprisePullRequestProperties Properties { get; set; }

        [JsonProperty(PropertyName = "reviewers")]
        public List<EnterpriseParticipant> Reviewers { get; set; }

        [JsonProperty(PropertyName = "participants")]
        public List<EnterpriseParticipant> Participants { get; set; }
    }
}
