namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IVsTools
    {
        void OpenTemporarySolution(string repositoryPath);
        bool OpenSolutionViaDlg(string repositoryPath);
    }
}