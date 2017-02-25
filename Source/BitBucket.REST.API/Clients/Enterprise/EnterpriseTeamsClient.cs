using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitBucket.REST.API.Helpers;
using BitBucket.REST.API.Interfaces;
using BitBucket.REST.API.Mappings;
using BitBucket.REST.API.Models.Enterprise;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.Wrappers;

namespace BitBucket.REST.API.Clients.Enterprise
{
    public class EnterpriseTeamsClient : ApiClient, ITeamsClient
    {
        public EnterpriseTeamsClient(EnterpriseBitbucketRestClient restClient, Connection connection) : base(restClient, connection)
        {

        }

        public Task<IteratorBasedPage<Team>> GetTeams() // not needed
        {
            //var url = EnterpriseApiUrls.Teams();
            //var teams = await RestClient.GetAllPages<string>(url, 100);
            var iterator = new IteratorBasedPage<Team>()
            {
                Next = null,
                Values = new List<Team>(),
                Page = 1,
                PageLen = 0,
                Size = 0
            };
            //return iterator;
            return Task.FromResult(iterator);
        }
    }
}