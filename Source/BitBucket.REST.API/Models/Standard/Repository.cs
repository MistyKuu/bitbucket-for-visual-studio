using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class Repository
    {
        [JsonProperty(PropertyName = "scm")]
        public string Scm { get; set; }

        [JsonProperty(PropertyName = "has_wiki")]
        public bool? HasWiki { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "links")]
        public Links Links { get; set; }

        [JsonProperty(PropertyName = "fork_policy")]
        public string ForkPolicy { get; set; }

        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }

        [JsonProperty(PropertyName = "created_on")]
        public string CreatedOn { get; set; }

        [JsonProperty(PropertyName = "full_name")]
        public string FullName { get; set; }

        [JsonProperty(PropertyName = "has_issues")]
        public bool? HasIssues { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public User Owner { get; set; }

        [JsonProperty(PropertyName = "updated_on")]
        public string UpdatedOn { get; set; }

        [JsonProperty(PropertyName = "size")]
        public ulong? Size { get; set; }

        [JsonProperty(PropertyName = "is_private")]
        public bool? IsPrivate { get; set; }

        [JsonProperty(PropertyName = "parent")]
        public Parent Parent { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}