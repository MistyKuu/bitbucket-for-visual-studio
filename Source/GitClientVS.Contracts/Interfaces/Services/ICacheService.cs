using GitClientVS.Contracts.Models;

namespace GitClientVS.Contracts.Interfaces.Services
{
    public interface ICacheService
    {
        void Add(string key, object value);
        Result<T> Get<T>(string key) where T : class;
    }
}