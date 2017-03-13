using System.Collections.Generic;
using System.Threading.Tasks;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using BitBucket.REST.API.QueryBuilders;

namespace BitBucket.REST.API.Interfaces
{
    public interface IUserClient
    {
        Task<User> GetUser();
    }
}