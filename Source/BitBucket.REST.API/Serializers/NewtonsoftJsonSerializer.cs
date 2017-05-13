using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;

namespace BitBucket.REST.API.Serializers
{
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _serializer;

        public NewtonsoftJsonSerializer()
        {
            ContentType = "application/json";
            _serializer = new Newtonsoft.Json.JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include
            };
        }

   
        public NewtonsoftJsonSerializer(Newtonsoft.Json.JsonSerializer serializer)
        {
            ContentType = "application/json";
            _serializer = serializer;
        }

        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = Formatting.Indented;
                    jsonTextWriter.QuoteChar = '"';

                    _serializer.Serialize(jsonTextWriter, obj);

                    return stringWriter.ToString();
                }
            }
        }

        public string DateFormat { get; set; }

        public T Deserialize<T>(IRestResponse response)
        {
            return this.Deserialize<T>(response.Content);
        }

        public T Deserialize<T>(string json)
        {
            using (var stringReader = new StringReader(json))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    return _serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }

        public string RootElement { get; set; }

        public string Namespace { get; set; }

        public string ContentType { get; set; }

    }
}