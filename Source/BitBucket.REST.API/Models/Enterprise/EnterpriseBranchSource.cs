using BitBucket.REST.API.Models.Standard;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class EnterpriseBranchSource : EnterpriseBranch
    {
        [JsonProperty(PropertyName = "repository")]
        public EnterpriseRepository Repository { get; set; }
    }
}