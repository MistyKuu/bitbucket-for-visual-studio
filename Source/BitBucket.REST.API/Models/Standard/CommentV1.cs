using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class CommentV1
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("line_From")]
        public long? LineFrom { get; set; }
        [JsonProperty("line_To")]
        public long? LineTo { get; set; }
        [JsonProperty("fileName")]
        public string FileName { get; set; }
        [JsonProperty("parent_id")]
        public long? ParentId { get; set; }
        [JsonProperty("content_rendered")]
        public string ContentRendered { get; set; }
        [JsonProperty(PropertyName = "utc_created_on")]
        public string CreatedOn { get; set; }
        [JsonProperty(PropertyName = "utc_last_updated")]
        public string UpdatedOn { get; set; }
        [JsonProperty(PropertyName = "deleted")]
        public bool Deleted { get; set; }
        [JsonProperty(PropertyName = "comment_id")]
        public long? CommentId { get; set; }
        [JsonProperty(PropertyName = "author_info")]
        public AuthorInfoV1 AuthorInfo { get; set; }
    }

    public class AuthorInfoV1
    {
        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }
        [JsonProperty(PropertyName = "display_Name")]
        public string DisplayName { get; set; }
        [JsonProperty(PropertyName = "avatar")]
        public string AvatarUrl { get; set; }
    }
}
