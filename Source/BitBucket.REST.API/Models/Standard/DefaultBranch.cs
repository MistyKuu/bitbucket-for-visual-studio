using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class DefaultBranch
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
