using System;
using BitBucket.REST.API.Serializers;
using RestSharp;

namespace BitBucket.REST.API.Wrappers
{
    public class BitbucketRestClient : RestClient
    {
        public BitbucketRestClient(Uri baseAddress) : base(baseAddress)
        {
            var serializer = new NewtonsoftJsonSerializer();
            this.AddHandler("application/json", serializer);
            this.AddHandler("text/json", serializer);
            this.AddHandler("text/x-json", serializer);
            this.AddHandler("text/javascript", serializer);
            this.AddHandler("*+json", serializer);
        }
        
    }
}