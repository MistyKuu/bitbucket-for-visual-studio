using GitClientVS.Contracts.Models;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IStorageService
    {
        Result SaveUserData(ConnectionData connectionData);
        Result<ConnectionData> LoadUserData();
        Result SaveProxySettings(ProxySettings proxySettings);
        Result<ProxySettings> LoadProxySettings();
    }
}