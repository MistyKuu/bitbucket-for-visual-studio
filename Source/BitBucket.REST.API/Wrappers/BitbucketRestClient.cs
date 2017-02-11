using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.QueryBuilders;
using BitBucket.REST.API.Serializers;
using RestSharp;

namespace BitBucket.REST.API.Wrappers
{
    public class BitbucketRestClient : BitbucketRestClientBase
    {
        public BitbucketRestClient(Connection connection) : base(connection)
        {
        }

        public override async Task<IteratorBasedPage<T>> GetAllPages<T>(string url, int limit = 100, IQueryConnector query = null)
        {
            var result = new IteratorBasedPage<T>()
            {
                Values = new List<T>()
            };
            IRestResponse<IteratorBasedPage<T>> response;
            var pageNumber = 1;
            do
            {
                var request = new BitbucketRestRequest(url, Method.GET);
                request.AddQueryParameter("pagelen", limit.ToString()).AddQueryParameter("page", pageNumber.ToString());
                if (query != null)
                {
                    request.AddQueryParameter("q", query.Build());
                }
                response = await this.ExecuteTaskAsync<IteratorBasedPage<T>>(request);
                if (response.Data.Values != null)
                {
                    result.Values.AddRange(response.Data.Values);
                }
                pageNumber++;
            } while (response.Data?.Next != null);//todo 99% this value should be used instead of pagenumber

            return result;
        }

        protected override string DeserializeError(IRestResponse response)
        {
            var serializer = new NewtonsoftJsonSerializer();
            return serializer.Deserialize<ErrorWrapper>(response.Content).Error.Message;
        }
    }
}