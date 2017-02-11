using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class Links
    {
        [JsonProperty(PropertyName = "self")]
        public Link Self { get; set; }

        [JsonProperty(PropertyName = "repositories")]
        public Link Repositories { get; set; }

        [JsonProperty(PropertyName = "link")]
        public Link Link { get; set; }

        [JsonProperty(PropertyName = "followers")]
        public Link Followers { get; set; }

        [JsonProperty(PropertyName = "avatar")]
        public Link Avatar { get; set; }

        [JsonProperty(PropertyName = "html")]
        public Link Html { get; set; }

        [JsonProperty(PropertyName = "following")]
        public Link Following { get; set; }

        [JsonProperty(PropertyName = "clone")]
        public List<Link> Clone { get; set; }
    }
}