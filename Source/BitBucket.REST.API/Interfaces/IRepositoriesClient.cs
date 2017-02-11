using System.Threading.Tasks;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.Interfaces
{
    public interface IRepositoriesClient
    {
        Task<IteratorBasedPage<Repository>> GetRepositories();
        Task<IteratorBasedPage<Repository>> GetRepositories(string owner);
        Task<IteratorBasedPage<Branch>> GetBranches(string repoName);
        Task<IteratorBasedPage<Branch>> GetBranches(string owner, string repoName);
        Task<Repository> CreateRepository(Repository repository);
    }
}