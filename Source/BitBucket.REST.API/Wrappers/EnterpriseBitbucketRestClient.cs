using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.QueryBuilders;
using BitBucket.REST.API.Serializers;
using RestSharp;

namespace BitBucket.REST.API.Wrappers
{
    public class EnterpriseBitbucketRestClient : BitbucketRestClientBase
    {
        public EnterpriseBitbucketRestClient(Connection connection) : base(connection)
        {
        }

        public override async Task<IteratorBasedPage<T>> GetAllPages<T>(string url, int limit = 100, IQueryConnector query = null)
        {
            var result = new EnterpriseIteratorBasedPage<T>()
            {
                Values = new List<T>()
            };
            IRestResponse<EnterpriseIteratorBasedPage<T>> response;
            ulong? pageNumber = 0;
            do
            {
                var request = new BitbucketRestRequest(url, Method.GET);
                request.AddQueryParameter("limit", limit.ToString()).AddQueryParameter("start", pageNumber.ToString());
                if (query != null)
                {
                    request.AddQueryParameter("q", query.Build());
                }
                response = await this.ExecuteTaskAsync<EnterpriseIteratorBasedPage<T>>(request);
                if (response.Data.Values != null)
                {
                    result.Values.AddRange(response.Data.Values);
                }

                pageNumber = response.Data.NextPageStart;

            } while (response.Data?.IsLastPage == false);

            return new IteratorBasedPage<T>()
            {//todo automapper
                Next = !result.IsLastPage.HasValue || result.IsLastPage.Value ? null : result.NextPageStart.ToString(),
                Page = result.Start + 1,
                PageLen = result.Limit,
                Size = result.Size,
                Values = result.Values
            };
        }

        protected override string DeserializeError(IRestResponse response)
        {
            var serializer = new NewtonsoftJsonSerializer();
            var deserialized = serializer.Deserialize<EnterpriseErrorWrapper>(response.Content);
            return string.Join(",", deserialized.Errors.Select(x => x.Message));
        }
    }
}