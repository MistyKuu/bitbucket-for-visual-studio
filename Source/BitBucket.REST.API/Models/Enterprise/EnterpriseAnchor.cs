using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class EnterpriseAnchor
    {
        [JsonProperty(PropertyName = "srcPath")]
        public string SourcePath { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "line")]
        public long? Line { get; set; }

        [JsonProperty(PropertyName = "lineType")]
        public string LineType { get; set; }

        [JsonProperty(PropertyName = "fileType")]
        public FileDiffType? FileType { get; set; }
    }
}