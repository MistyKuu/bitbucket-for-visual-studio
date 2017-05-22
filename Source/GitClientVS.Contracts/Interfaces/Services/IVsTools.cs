namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IVsTools
    {
        void RunDiff(string content1, string content2, string fileDisplayName1, string fileDisplayName2);
    }
}