using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class EnterpriseBrowseText
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }
}