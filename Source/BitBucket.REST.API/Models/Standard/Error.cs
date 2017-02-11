using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class Error
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}