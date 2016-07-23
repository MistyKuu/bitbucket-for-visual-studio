using BitBucket.REST.API.Models;
using BitBucket.REST.API.Wrappers;

namespace BitBucket.REST.API.Clients
{
    public class PullRequestsClient : ApiClient
    {
        public PullRequestsClient(BitbucketRestClient restClient, Connection connection) : base(restClient, connection)
        {
            
        }

    }
}