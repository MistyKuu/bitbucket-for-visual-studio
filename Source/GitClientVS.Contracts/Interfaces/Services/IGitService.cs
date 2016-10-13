using GitClientVS.Contracts.Models.GitClientModels;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IGitService
    {
        GitRepository GetActiveRepository();
        void CloneRepository(string cloneUrl, string repositoryName, string repositoryPath);
        void PublishRepository(GitRepository repository);
        string GetActiveBranchFromActiveRepository();
        string GetHeadCommitOfActiveBranch();
    }
}