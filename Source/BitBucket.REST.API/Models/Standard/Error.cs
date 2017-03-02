using Newtonsoft.Json;
using System.Collections.Generic;

namespace BitBucket.REST.API.Models.Standard
{
    public class Error
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "fields")]
        public ErrorField Fields { get; set; }
    }
}