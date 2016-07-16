using GitClientVS.Contracts.Models;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IStorageService
    {
        Result SaveUserData(ConnectionData connectionData);
        Result<ConnectionData> LoadUserData();
    }
}