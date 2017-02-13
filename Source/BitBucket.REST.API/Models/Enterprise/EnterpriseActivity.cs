using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class EnterpriseActivity
    {
        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        [JsonProperty(PropertyName = "commentAction")]
        public string CommentAction { get; set; }

        [JsonProperty(PropertyName = "user")]
        public EnterpriseUser User { get; set; }
    }
}