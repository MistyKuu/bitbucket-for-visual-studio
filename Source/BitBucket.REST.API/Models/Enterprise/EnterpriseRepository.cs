using BitBucket.REST.API.Models.Standard;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class EnterpriseRepository
    {
        [JsonProperty(PropertyName = "scmId")]
        public string Scm { get; set; }

        [JsonProperty(PropertyName = "has_wiki")]
        public bool? HasWiki { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "links")]
        public EnterpriseLinks Links { get; set; }

        [JsonProperty(PropertyName = "forkable")]
        public bool? Forkable { get; set; }

        [JsonProperty(PropertyName = "public")]
        public bool? IsPublic { get; set; }

        [JsonProperty(PropertyName = "project")]
        public EnterpriseProject Project { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}