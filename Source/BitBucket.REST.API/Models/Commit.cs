using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models
{
    public class Commit
    {
        [JsonProperty(PropertyName = "hash")]
        public string Hash { get; set; }

        [JsonProperty(PropertyName = "links")]
        public Links Links { get; set; }

        [JsonProperty(PropertyName = "repository")]
        public Repository Repository { get; set; }

        [JsonProperty(PropertyName = "author")]
        public Author Author { get; set; }

        [JsonProperty(PropertyName = "participants")]
        public List<User> Participants { get; set; }

        [JsonProperty(PropertyName = "parents")]
        public List<Parent> Parents { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}