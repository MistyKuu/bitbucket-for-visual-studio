using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class Comment 
    {
        [JsonProperty(PropertyName = "user")]
        public User User { get; set; }

        [JsonProperty(PropertyName = "content")]
        public Content Content { get; set; }

        [JsonProperty(PropertyName = "created_on")]
        public string CreatedOn { get; set; }

        [JsonProperty(PropertyName = "updated_on")]
        public string UpdatedOn { get; set; }

        [JsonProperty(PropertyName = "parent")]
        public Parent Parent { get; set; }

        [JsonProperty(PropertyName = "inline")]
        public Inline Inline { get; set; }

        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        public bool IsDeleted { get; set; }
        public long Version { get; set; }
    }
}