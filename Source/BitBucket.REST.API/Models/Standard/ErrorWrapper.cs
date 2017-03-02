using Newtonsoft.Json;
using System.Collections.Generic;

namespace BitBucket.REST.API.Models.Standard
{
    public class ErrorWrapper
    {
        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }
    }
}