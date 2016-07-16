using Newtonsoft.Json.Serialization;

namespace BitBucket.REST.API.Serializers
{
    public class BitbucketPropertyNamesContractResolver : DefaultContractResolver
    {
        public BitbucketPropertyNamesContractResolver() : base(true)
        {
            
        }

        protected override string ResolvePropertyName(string propertyName)
        {


            return null;
        }

    }
}