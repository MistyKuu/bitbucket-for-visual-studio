using System.Threading.Tasks;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.Wrappers;
using RestSharp;

namespace BitBucket.REST.API.Clients
{
    public class TeamsClient : ApiClient, ITeamsClient
    {
        public TeamsClient(BitbucketRestClient restClient, Connection connection) : base(restClient, connection)
        {

        }

        public async Task<IteratorBasedPage<Team>> GetTeams()
        {
            var url = ApiUrls.Teams();
            return await RestClient.GetAllPages<Team>(url);
        }
    }
}