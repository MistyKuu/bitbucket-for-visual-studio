namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IGitService
    {
        void CloneRepository(string cloneUrl, string repositoryName, string repositoryPath);
    }
}