using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class EnterpriseErrorWrapper
    {
        [JsonProperty(PropertyName = "errors")]
        public List<EnterpriseError> Errors { get; set; }
    }
}