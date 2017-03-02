using Newtonsoft.Json;
using System.Collections.Generic;

namespace BitBucket.REST.API.Models.Standard
{
    public class ErrorField
    {
        [JsonProperty(PropertyName = "source")]
        public List<string> Source { get; set; }
    }
}