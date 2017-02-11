using System.Threading.Tasks;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;

namespace BitBucket.REST.API.Interfaces
{
    public interface IUserClient
    {
        Task<User> GetUser();
    }
}