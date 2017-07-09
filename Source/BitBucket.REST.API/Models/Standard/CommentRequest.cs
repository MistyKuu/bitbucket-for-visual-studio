using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class CommentRequest
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("lineFrom")]
        public long? LineFrom { get; set; }
        [JsonProperty("lineTo")]
        public long? LineTo { get; set; }
        [JsonProperty("fileName")]
        public string FileName { get; set; }
        [JsonProperty("parent_id")]
        public long? ParentId { get; set; }
    }
}
