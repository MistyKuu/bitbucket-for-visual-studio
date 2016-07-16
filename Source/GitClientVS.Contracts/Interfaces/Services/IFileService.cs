namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IFileService
    {
        void Save(string path, string content);
        string Read(string path);
    }
}