using System.Collections.Generic;
using BitBucket.REST.API.Models.Standard;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class EnterpriseLinks
    {
        [JsonProperty(PropertyName = "repositories")]
        public List<EnterpriseLink> Repositories { get; set; }

        [JsonProperty(PropertyName = "link")]
        public List<EnterpriseLink> Link { get; set; }

        [JsonProperty(PropertyName = "followers")]
        public List<EnterpriseLink> Followers { get; set; }

        [JsonProperty(PropertyName = "avatar")]
        public List<EnterpriseLink> Avatar { get; set; }

        [JsonProperty(PropertyName = "html")]
        public List<EnterpriseLink> Html { get; set; }

        [JsonProperty(PropertyName = "following")]
        public List<EnterpriseLink> Following { get; set; }

        [JsonProperty(PropertyName = "clone")]
        public List<EnterpriseLink> Clone { get; set; }

        [JsonProperty(PropertyName = "self")]
        public List<EnterpriseLink> Self { get; set; }
    }
}