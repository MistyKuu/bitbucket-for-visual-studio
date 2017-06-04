using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class RepositoryV1
    {
        [JsonProperty("scm")]
        public string Scm { get; set; }
        [JsonProperty("has_wiki")]
        public bool HasWiki { get; set; }
        [JsonProperty("last_updated")]
        public DateTime LastUpdated { get; set; }
        [JsonProperty("no_forks")]
        public bool NoForks { get; set; }
        [JsonProperty("created_on")]
        public DateTime CreatedOn { get; set; }
        [JsonProperty("owner")]
        public string Owner { get; set; }
        [JsonProperty("logo")]
        public string Logo { get; set; }
        [JsonProperty("email_mailinglist")]
        public string EmailMailinglist { get; set; }
        [JsonProperty("is_mq")]
        public bool IsMq { get; set; }
        [JsonProperty("size")]
        public ulong Size { get; set; }
        [JsonProperty("read_only")]
        public bool ReadOnly { get; set; }
        [JsonProperty("fork_of")]
        public object ForkOf { get; set; }
        [JsonProperty("mq_of")]
        public object MqOf { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("utc_created_on")]
        public string UtcCreatedOn { get; set; }
        [JsonProperty("website")]
        public string Website { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("has_issues")]
        public bool HasIssues { get; set; }
        [JsonProperty("is_fork")]
        public bool IsFork { get; set; }
        [JsonProperty("slug")]
        public string Slug { get; set; }
        [JsonProperty("is_private")]
        public bool IsPrivate { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("utc_last_updated")]
        public string UtcLastUpdated { get; set; }
        [JsonProperty("no_public_forks")]
        public bool NoPublicForks { get; set; }
        [JsonProperty("creator")]
        public object Creator { get; set; }
        [JsonProperty("resource_uri")]
        public string ResourceUri { get; set; }
    }

}
