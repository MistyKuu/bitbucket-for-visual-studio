using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class UserShort
    {
        [JsonProperty(PropertyName = "username")]
        public string FromUserName { get; set; }

        [JsonProperty(PropertyName = "nickname")]
        public string FromNickName { get; set; }

        public string Username => !string.IsNullOrEmpty(FromUserName) ? FromUserName : FromNickName;

        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "links")]
        public Links Links { get; set; }

        public string Email { get; set; }
    }
}