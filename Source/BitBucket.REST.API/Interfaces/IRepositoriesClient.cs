using System.Threading.Tasks;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using System.Collections.Generic;

namespace BitBucket.REST.API.Interfaces
{
    public interface IRepositoriesClient
    {
        Task<IEnumerable<Repository>> GetRepositories();
        Task<IEnumerable<Repository>> GetRepositories(string owner);
        Task<IEnumerable<Branch>> GetBranches(string repoName);
        Task<IEnumerable<Branch>> GetBranches(string owner, string repoName);
        Task<Repository> CreateRepository(Repository repository);
        Task<Commit> GetCommitById(string repoName, string owner, string id);
        Task<IEnumerable<Commit>> GetCommitsRange(string repoName, string owner, Branch fromBranch, Branch toBranch);
    }
}