using System.Collections.Generic;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Enterprise
{
    public class Hunk
    {
        [JsonProperty(PropertyName = "sourceLine")]
        public int SourceLine { get; set; }

        [JsonProperty(PropertyName = "sourceSpan")]
        public int SourceSpan { get; set; }

        [JsonProperty(PropertyName = "destinationLine")]
        public int DestinationLine { get; set; }

        [JsonProperty(PropertyName = "destinationSpan")]
        public int DestinationSpan { get; set; }

        [JsonProperty(PropertyName = "segments")]
        public List<Segment> Segments { get; set; }

        [JsonProperty(PropertyName = "truncated")]
        public bool Truncated { get; set; }
    }
}