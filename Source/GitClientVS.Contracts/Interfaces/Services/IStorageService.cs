using GitClientVS.Contracts.Models;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface IStorageService
    {
        Result SaveUserData(CombinedConnectionData connectionData);
        Result<CombinedConnectionData> LoadUserData();
        Result SaveProxySettings(ProxySettings proxySettings);
        Result<ProxySettings> LoadProxySettings();
    }
}