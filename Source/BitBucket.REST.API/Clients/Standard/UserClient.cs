using System.Collections.Generic;
using System.Threading.Tasks;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.QueryBuilders;
using BitBucket.REST.API.Wrappers;
using RestSharp;

namespace BitBucket.REST.API.Clients.Standard
{
    public class UserClient : ApiClient, IUserClient
    {

        public UserClient(IBitbucketRestClient restClient, Connection connection) : base(restClient, connection)
        {

        }

        public async Task<User> GetUser()
        {
            var url = ApiUrls.User();
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync<User>(request);
            return response.Data;
        }

        public async Task<User> GetUser(string id)
        {
            var url = ApiUrls.User(id);
            var request = new BitbucketRestRequest(url, Method.GET);
            var response = await RestClient.ExecuteTaskAsync<User>(request);
            return response.Data;
        }
    }
}