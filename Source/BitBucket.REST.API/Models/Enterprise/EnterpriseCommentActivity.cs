using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class EnterpriseCommentActivity : EnterpriseActivity
    {
        [JsonProperty(PropertyName = "comment")]
        public EnterpriseComment Comment { get; set; }
    }
}