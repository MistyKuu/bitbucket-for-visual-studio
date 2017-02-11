using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class CommentEnterpriseActivity : EnterpriseActivity
    {
        [JsonProperty(PropertyName = "comment")]
        public EnterpriseComment Comment { get; set; }
    }
}