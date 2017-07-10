using BitBucket.REST.API.Models.Standard;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class EnterpriseParent
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }
}