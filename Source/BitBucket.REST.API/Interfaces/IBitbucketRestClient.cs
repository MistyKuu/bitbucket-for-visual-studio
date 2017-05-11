using System.Collections.Generic;
using System.Threading.Tasks;
using BitBucket.REST.API.QueryBuilders;
using RestSharp;

namespace BitBucket.REST.API.Interfaces
{
    public interface IBitbucketRestClient
    {
        Task<IEnumerable<T>> GetAllPages<T>(string url, int limit = 50, QueryString query = null);
        Task<IRestResponse<T>> ExecuteTaskAsync<T>(IRestRequest request);
        Task<IRestResponse> ExecuteTaskAsync(IRestRequest request);
    }
}