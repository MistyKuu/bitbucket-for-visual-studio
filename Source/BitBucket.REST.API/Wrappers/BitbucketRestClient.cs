using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.QueryBuilders;
using BitBucket.REST.API.Serializers;
using RestSharp;
using System.Linq;
using BitBucket.REST.API.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BitBucket.REST.API.Wrappers
{
    public class BitbucketRestClient : BitbucketRestClientBase, IBitbucketRestClient
    {
        public BitbucketRestClient(Connection connection) : base(connection)
        {
        }

        public override async Task<IEnumerable<T>> GetAllPages<T>(string url, int limit = 50, QueryString query = null)
        {
            var result = new IteratorBasedPage<T>()
            {
                Values = new List<T>()
            };

            var request = new BitbucketRestRequest(url, Method.GET);

            request.AddQueryParameter("pagelen", limit.ToString());

            if (query != null)
                foreach (var par in query)
                    request.AddQueryParameter(par.Key, par.Value);

            var response = await this.ExecuteTaskAsync<IteratorBasedPage<T>>(request);

            if (response.Data?.Values != null)
                result.Values.AddRange(response.Data.Values);


            while (response.Data?.Next != null)
            {
                request = new BitbucketRestRequest(response.Data.Next, Method.GET);
                response = await this.ExecuteTaskAsync<IteratorBasedPage<T>>(request);
                if (response.Data?.Values != null)
                    result.Values.AddRange(response.Data.Values);
            }

            return result.Values;
        }

        protected override string DeserializeError(IRestResponse response)
        {
            var errorObject = JObject.Parse(response.Content);

            var error = (JObject)errorObject["error"];
            var message = error["message"].Value<string>();
            var fields = error["fields"]?.ToList().Select(x => x.ToString()).ToList() ?? new List<string>();
            message += ". " + string.Join(", ", fields);

            return message;
        }
    }
}