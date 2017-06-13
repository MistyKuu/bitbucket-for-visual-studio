using GitClientVS.Contracts.Models;
using GitClientVS.Contracts.Models.GitClientModels;
using System.Collections.Generic;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IGitService
    {
        GitRemoteRepository GetActiveRepository();
        void CloneRepository(string cloneUrl, string repositoryName, string repositoryPath);
        void PublishRepository(GitRemoteRepository repository);
        string GetDefaultRepoPath();
        IEnumerable<LocalRepo> GetLocalRepositories();
    }
}