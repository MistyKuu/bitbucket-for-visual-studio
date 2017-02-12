using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class EnterprisePullRequestProperties
    {
        [JsonProperty(PropertyName = "commentCount")]
        public int CommentsCount { get; set; } //todo more props if necessary
    }
}