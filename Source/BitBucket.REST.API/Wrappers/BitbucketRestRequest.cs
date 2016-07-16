using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitBucket.REST.API.Serializers;
using RestSharp;

namespace BitBucket.REST.API.Wrappers
{
    class BitbucketRestRequest : RestRequest
    {
        public BitbucketRestRequest(string resource, Method method) : base(resource, method)
        {
            this.JsonSerializer = new NewtonsoftJsonSerializer();
            
        }
    }
}
