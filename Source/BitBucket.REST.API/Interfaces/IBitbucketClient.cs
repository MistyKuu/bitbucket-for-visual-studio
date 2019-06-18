using System.Threading.Tasks;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using RestSharp;

namespace BitBucket.REST.API.Interfaces
{
    public interface IBitbucketClient
    {
        ITeamsClient TeamsClient { get; }
        IRepositoriesClient RepositoriesClient { get; }
        IUserClient UserClient { get; }
        IPullRequestsClient PullRequestsClient { get; }
        Connection ApiConnection { get; }
        BitBucketType BitBucketType { get; }
    }
}