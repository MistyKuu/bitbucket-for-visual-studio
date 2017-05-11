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
        public EnterpriseTeamsClient(IEnterpriseBitbucketRestClient restClient, Connection connection) : base(restClient, connection)
        {

        }

        public Task<IEnumerable<Team>> GetTeams() // not needed
        {
            IEnumerable<Team> coll = new List<Team>();
            return Task.FromResult(coll);
        }
    }
}