using System.Collections.Generic;
using BitBucket.REST.API.Models.Standard;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class EnterpriseComment
    {
        [JsonProperty(PropertyName = "author")]
        public EnterpriseUser User { get; set; }
        [JsonProperty(PropertyName = "createdDate")]
        public long CreatedOn { get; set; }

        [JsonProperty(PropertyName = "updatedDate")]
        public long UpdatedOn { get; set; }

        [JsonProperty(PropertyName = "comments")]
        public List<EnterpriseComment> Comments { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        public EnterpriseParent Parent { get; set; }
        public EnterpriseAnchor Anchor { get; set; }
    }
}