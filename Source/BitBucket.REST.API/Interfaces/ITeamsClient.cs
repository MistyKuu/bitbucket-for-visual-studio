using System.Threading.Tasks;
using BitBucket.REST.API.Models;
using BitBucket.REST.API.Models.Standard;
using System.Collections.Generic;

namespace BitBucket.REST.API.Interfaces
{
    public interface ITeamsClient
    {
        Task<IEnumerable<Team>> GetTeams();
    }
}