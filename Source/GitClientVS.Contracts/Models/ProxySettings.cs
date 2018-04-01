using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitClientVS.Contracts.Models
{
    public class ProxySettings
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string Password { get; set; }
    }
}
