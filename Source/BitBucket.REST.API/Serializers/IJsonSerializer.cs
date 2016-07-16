using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace BitBucket.REST.API.Serializers
{
    public interface IJsonSerializer : ISerializer, IDeserializer
    {
        
    }
}