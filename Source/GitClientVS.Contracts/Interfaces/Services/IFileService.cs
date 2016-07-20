using GitClientVS.Contracts.Models;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IFileService
    {
        void Save(string path, string content);
        string Read(string path);
        Result<string> OpenSaveDialog(string filterPattern);
        Result<string> OpenDirectoryDialog(string selectedPath, string title = null);
    }
}