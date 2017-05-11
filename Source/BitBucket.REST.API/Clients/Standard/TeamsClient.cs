using System.Threading.Tasks;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.Wrappers;
using System.Collections.Generic;

namespace BitBucket.REST.API.Clients.Standard
{
    public class TeamsClient : ApiClient, ITeamsClient
    {
        public TeamsClient(IBitbucketRestClient restClient, Connection connection) : base(restClient, connection)
        {

        }

        public async Task<IEnumerable<Team>> GetTeams()
        {
            var url = ApiUrls.Teams();
            return await RestClient.GetAllPages<Team>(url);
        }
    }
}