﻿using Newtonsoft.Json;

namespace BitBucket.REST.API.Models.Standard
{
    public class ErrorWrapper
    {
        [JsonProperty(PropertyName = "error")]
        public Error Error { get; set; }
    }
}